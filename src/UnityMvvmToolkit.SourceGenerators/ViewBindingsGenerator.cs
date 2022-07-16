using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityMvvmToolkit.SourceGenerators.Captures;
using UnityMvvmToolkit.SourceGenerators.Extensions;
using UnityMvvmToolkit.SourceGenerators.Models;
using UnityMvvmToolkit.SourceGenerators.SyntaxReceivers;

namespace UnityMvvmToolkit.SourceGenerators;

[Generator]
public class ViewBindingsGenerator : ISourceGenerator
{
    private const string AssetsFolderName = "Assets";
    private const string BindableIdentifier = "Bindable";
    
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new ViewBindingsReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not ViewBindingsReceiver receiver)
        {
            return;
        }

        foreach (var view in receiver.Views.Captures)
        {
            var assetFullPath = GetVisualTreeAssetPath(view);
            if (File.Exists(assetFullPath) == false)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        "LG01",
                        "View bindings generator",
                        "Visual tree asset file is not found.",
                        defaultSeverity: DiagnosticSeverity.Error,
                        severity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true,
                        warningLevel: 0));

                return;
            }

            var bindingClasses = new Dictionary<string, (string, string)>();
            var visualTreeElements = GetBindableElements(assetFullPath);

            var index = 0;
            foreach (var visualTreeElement in visualTreeElements)
            {
                if (receiver.BindableElements.Captures.TryGetValue(visualTreeElement.ClassIdentifier,
                        out var visualElement) == false)
                {
                    continue;
                }

                var classIdentifier = visualElement.ClassIdentifier;
                var classIdentifierIndexed = $"{classIdentifier}{index}";
                var classBody =
                    CreateElementBindingsClass(classIdentifierIndexed, view.ViewModelIdentifier, visualElement,
                        visualTreeElement);

                if (classBody == null)
                {
                    continue;
                }

                index++;
                bindingClasses.Add(classIdentifier, (classIdentifierIndexed, classBody));
            }

            if (bindingClasses.Count != 0)
            {
                context.AddSource($"{view.Class.Identifier.Text}.g.cs", GenerateView(context, view, bindingClasses));
            }
        }
    }

    private string CreateElementBindingsClass(string classIdentifier, string viewModelIdentifier, 
        BindableElementCapture visualElement, VisualTreeElementInfo visualTreeElement)
    {
        var valueAssignments = GetUpdateValueMethodBody(visualElement.Properties, visualTreeElement);
        if (valueAssignments.Count == 0)
        {
            return null;
        }

        return $@"
        private class {classIdentifier}Bindings : IVisualElementBindings
        {{
            private readonly {viewModelIdentifier} _viewModel;
            private readonly {visualElement.ClassIdentifier} _visualElement;

            public {classIdentifier}Bindings({viewModelIdentifier} viewModel, {visualElement.ClassIdentifier} visualElement)
            {{
                _viewModel = viewModel;
                _visualElement = visualElement;
            }}

            public void UpdateValues()
            {{
                {string.Join(Environment.NewLine, valueAssignments)}
            }}
        }}";
    }

    private List<string> GetUpdateValueMethodBody(IReadOnlyDictionary<string, string> visualElementProperties,
        VisualTreeElementInfo visualTreeElement)
    {
        var valueAssignments = new List<string>();

        foreach (var visualTreeAttribute in visualTreeElement.Attributes)
        {
            var attributeName = visualTreeAttribute.Key.Replace("-", "");
            if (visualElementProperties.TryGetValue(attributeName, out var targetPropertyName) == false)
            {
                continue;
            }

            valueAssignments.Add($@"if (_visualElement.{targetPropertyName} != _viewModel.{visualTreeAttribute.Value})
                {{
                    _visualElement.{targetPropertyName} = _viewModel.{visualTreeAttribute.Value};
                }}");
        }

        return valueAssignments;
    }
    
    private string GenerateView(GeneratorExecutionContext context, ViewCapture view,
        Dictionary<string, (string, string)> bindingClasses)
    {
        var usingDirectives = view.Class.SyntaxTree.GetRoot().DescendantNodes()
            .OfType<UsingDirectiveSyntax>();
        var usingDirectivesAsText = string.Join(Environment.NewLine, usingDirectives);
            
        // TODO: Filter using directives.

        var switchStringBuilder = new StringBuilder();
        var classesStringBuilder = new StringBuilder();
        
        foreach (var @class in bindingClasses)
        {
            switchStringBuilder.Append($"{@class.Key} visualElement => new {@class.Value.Item1}Bindings(bindingContext, visualElement),");
            switchStringBuilder.Append($"{Environment.NewLine}\t\t\t\t");
            
            classesStringBuilder.Append(@class.Value.Item2);
            classesStringBuilder.Append($"{Environment.NewLine}");
        }
        
        return $@"using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.BindableVisualElements;

{usingDirectivesAsText}

namespace {view.Class.GetParent<NamespaceDeclarationSyntax>().Name}
{{
    public partial class {view.Class.Identifier.Text}
    {{
        protected override IVisualElementBindings GetVisualElementsBindings(MainMenuViewModel bindingContext,
            IBindableVisualElement bindableElement)
        {{
            return bindableElement switch
            {{
                {switchStringBuilder.ToString().TrimEnd()}
                _ => default
            }};
        }}
        {classesStringBuilder.ToString().TrimEnd()}
    }}
}}";
    }

    private IEnumerable<VisualTreeElementInfo> GetBindableElements(string assetFullPath)
    {
        var result = new List<VisualTreeElementInfo>();

        var xmlDocument = new XmlDocument();
        xmlDocument.Load(assetFullPath);

        if (xmlDocument.DocumentElement == null)
        {
            return result;
        }
        
        foreach (XmlNode node in xmlDocument.DocumentElement.ChildNodes)
        {
            if (node.Name.Contains(BindableIdentifier) == false)
            {
                continue;
            }

            if (node.Attributes == null)
            {
                continue;
            }
            
            var bindableElementInfo = new VisualTreeElementInfo(node.Name.Split('.').Last());

            foreach (XmlAttribute attribute in node.Attributes)
            {
                bindableElementInfo.Attributes.Add(new KeyValuePair<string, string>(attribute.Name, attribute.Value));
            }

            result.Add(bindableElementInfo);
        }

        return result;
    }

    private string GetVisualTreeAssetPath(ViewCapture view)
    {
        if (view.Class.SyntaxTree.HasCompilationUnitRoot == false)
        {
            return null;
        }

        var viewDirectory = Path.GetDirectoryName(view.Class.SyntaxTree.FilePath);
        var assetsFolderIndex = viewDirectory?.IndexOf(AssetsFolderName, StringComparison.Ordinal);
        
        return assetsFolderIndex is null or -1
            ? null
            : Path.Combine(viewDirectory.Substring(0, assetsFolderIndex.Value + AssetsFolderName.Length),
                view.AssetPath);
    }

    private string GenerateBody()
    {
        return
            $@"protected override IVisualElementBindings GetVisualElementsBindings(MainMenuViewModel bindingContext,
            IBindableVisualElement bindableElement)
        {{
            return bindableElement switch
            {{
                BindableLabel bindableLabel => new LabelBindings(bindingContext, bindableLabel),
                BindableTextField bindableTextField => new TextFieldBindings(bindingContext, bindableTextField),
                _ => default
            }};
        }}

        private class LabelBindings : IVisualElementBindings
        {{
            private readonly MainMenuViewModel _viewModel;
            private readonly BindableLabel _visualElement;

            public LabelBindings(MainMenuViewModel viewModel, BindableLabel visualElement)
            {{
                _viewModel = viewModel;
                _visualElement = visualElement;
            }}

            public void UpdateValues()
            {{
                UpdateStrValue();
            }}

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void UpdateStrValue()
            {{
                if (_visualElement.text != _viewModel.StrValue)
                {{
                    _visualElement.text = _viewModel.StrValue;
                }}
            }}
        }}

        private class TextFieldBindings : IVisualElementBindings, IDisposable
        {{
            private readonly MainMenuViewModel _viewModel;
            private readonly BindableTextField _visualElement;

            public TextFieldBindings(MainMenuViewModel viewModel, BindableTextField visualElement)
            {{
                _viewModel = viewModel;

                _visualElement = visualElement;
                _visualElement.RegisterValueChangedCallback(OnVisualElementValueChanged);
            }}

            public void Dispose()
            {{
                _visualElement.UnregisterValueChangedCallback(OnVisualElementValueChanged);
            }}

            public void UpdateValues()
            {{
                UpdateStrValue();
            }}

            private void OnVisualElementValueChanged(ChangeEvent<string> e)
            {{
                _viewModel.StrValue = e.newValue;
            }}

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void UpdateStrValue()
            {{
                if (_visualElement.value != _viewModel.StrValue)
                {{
                    _visualElement.SetValueWithoutNotify(_viewModel.StrValue);
                }}
            }}
        }}";
    }
}