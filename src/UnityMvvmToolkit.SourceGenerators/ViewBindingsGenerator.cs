using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityMvvmToolkit.SourceGenerators.Captures;
using UnityMvvmToolkit.SourceGenerators.Extensions;
using UnityMvvmToolkit.SourceGenerators.Models;
using UnityMvvmToolkit.SourceGenerators.SyntaxReceivers;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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

        // var sb = new StringBuilder();
        // sb.Append(receiver.Views.Captures.Count());
        // sb.AppendLine();
        // sb.Append(receiver.BindableElements.Captures.Values.Count());
        //
        // File.WriteAllText($@"D:\0.txt", sb.ToString());
        // return;

        // if (receiver.Views.Captures.Count() > 0)
        // {
        //     File.WriteAllText($@"D:\0.txt", receiver.Views.Captures.First().Class.SyntaxTree.GetRoot().GetParent<NamespaceDeclarationSyntax>().Name.ToString());
        // }
        //
        // if (receiver.BindableElements.Captures.Values.Count() > 0)
        // {
        //     File.WriteAllText($@"D:\1.txt", receiver.BindableElements.Captures.Values.First().Class.SyntaxTree.GetRoot().GetParent<NamespaceDeclarationSyntax>().Name.ToString());
        // }
        //
        // if ((receiver.Views.Captures.Count() > 0)
        //     &&
        //     (receiver.BindableElements.Captures.Values.Count() > 0)
        //    )
        // {
        //     File.WriteAllText($@"D:\2.txt", "!!!");
        // }
        //
        // // File.WriteAllText($@"D:\0.txt", receiver.Views.Captures.Count().ToString());
        // // File.WriteAllText($@"D:\1.txt", receiver.BindableElements.Captures.Values.Count().ToString());
        //
        // return;
        
        foreach (var bindableElement in receiver.BindableElements.Captures.Values)
        {
            File.WriteAllText($@"D:\{bindableElement.Class.Identifier.Text}.txt", bindableElement.Class.GetText(Encoding.UTF8).ToString());
        }
        
        var compilation = context.Compilation;

        foreach (var view in receiver.Views.Captures)
        {
            // foreach (var bindableElement in receiver.BindableElements.Captures.Values)
            // {
            //     File.WriteAllText($@"D:\{bindableElement.Class.Identifier.Text}.txt", bindableElement.Class.GetText(Encoding.UTF8).ToString());
            // }

            File.WriteAllText($@"D:\{view.Class.Identifier.Text}.txt", view.Class.GetText(Encoding.UTF8).ToString());
            
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
            
            var bindableElements = GetBindableElements(assetFullPath);

            foreach (var bindableElement in bindableElements)
            {
                if (receiver.BindableElements.Captures.TryGetValue(bindableElement.ClassIdentifier,
                        out var visualElement) == false)
                {
                    continue;
                }
                
                var elementBindingsClass = CreateElementBindingsClass(visualElement, view.ViewModelIdentifier);
                File.WriteAllText($@"D:\{visualElement.Class.Identifier.Text}Bindings.txt", elementBindingsClass);
            }
            
            // var sb = new StringBuilder();
            // foreach (var bindableElement in bindableElements)
            // {
            //     sb.Append(bindableElement.ClassIdentifier);
            //     sb.AppendLine();
            //
            //     foreach (var attribute in bindableElement.Attributes)
            //     {
            //         sb.Append($"{attribute.Key} - {attribute.Value}");
            //         sb.AppendLine();
            //     }
            //
            //     sb.AppendLine();
            // }
            //
            // File.WriteAllText($@"D:\BindableElements.txt", sb.ToString());
            
            
            
            // var sb = new StringBuilder();
            // foreach (var visualElement in receiver.BindableElements.Captures)
            // {
            //     sb.Append(visualElement.Key);
            //     sb.AppendLine();
            // }
            
            File.WriteAllText($@"D:\BindableElements.txt", receiver.BindableElements.Captures.Values.Count().ToString());
            
            foreach (var bindableElement in receiver.BindableElements.Captures.Values)
            {
                File.WriteAllText($@"D:\{bindableElement.Class.Identifier.Text}.txt", bindableElement.Class.GetText(Encoding.UTF8).ToString());
            }
            
            var syntaxTree = view.Class.SyntaxTree;

            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var immutableHashSet = syntaxTree.GetRoot()
                .DescendantNodesAndSelf()
                .OfType<ClassDeclarationSyntax>()
                .Select(x => ModelExtensions.GetDeclaredSymbol(semanticModel, x))
                .OfType<ITypeSymbol>()
                // .Where(x => x.Interfaces.Contains(viewInterface))
                .ToImmutableHashSet();
                
            foreach (var typeSymbol in immutableHashSet)
            {
                context.AddSource($"{typeSymbol.Name}.g.cs", GenerateView(typeSymbol, context, view));
            }

            // var elementBindingsClass = CreateElementBindingsClass();
            // var output = elementBindingsClass.GetText(Encoding.UTF8).ToString();
        }
    }

    private string CreateElementBindingsClass(BindableElementCapture bindableElementCapture, string viewViewModelIdentifier)
    {
        var classIdentifier = $"{bindableElementCapture.Class.Identifier.Text}Bindings";

        return $@"
private class {classIdentifier} : IVisualElementBindings
{{
    private readonly {viewViewModelIdentifier} _viewModel;
    private readonly BindableLabel _visualElement;

    public {classIdentifier}({viewViewModelIdentifier} viewModel, BindableLabel visualElement)
    {{
        _viewModel = viewModel;
        _visualElement = visualElement;
    }}

    public void UpdateValues()
    {{
        {Block(GetUpdateValueMethodBody(bindableElementCapture.Properties)).GetText(Encoding.UTF8)}
    }}
}}";

    // return ClassDeclaration(classIdentifier)
    //         .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword))).WithBaseList(
    //             BaseList(SingletonSeparatedList<BaseTypeSyntax>(
    //                 SimpleBaseType(IdentifierName("IVisualElementBindings")))))
    //         .WithMembers(List(new MemberDeclarationSyntax[]
    //         {
    //             FieldDeclaration(VariableDeclaration(IdentifierName("MainMenuViewModel"))
    //                     .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_viewModel")))))
    //                 .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
    //             FieldDeclaration(VariableDeclaration(IdentifierName("BindableLabel"))
    //                     .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_visualElement")))))
    //                 .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
    //             ConstructorDeclaration(Identifier(classIdentifier))
    //                 .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
    //                 .WithParameterList(
    //                     ParameterList(
    //                         SeparatedList<ParameterSyntax>(
    //                             new SyntaxNodeOrToken[]
    //                             {
    //                                 Parameter(
    //                                         Identifier("viewModel"))
    //                                     .WithType(
    //                                         IdentifierName("MainMenuViewModel")),
    //                                 Token(SyntaxKind.CommaToken),
    //                                 Parameter(
    //                                         Identifier("visualElement"))
    //                                     .WithType(
    //                                         IdentifierName("BindableLabel"))
    //                             })))
    //                 .WithBody(
    //                     Block(
    //                         ExpressionStatement(
    //                             AssignmentExpression(
    //                                 SyntaxKind.SimpleAssignmentExpression,
    //                                 IdentifierName("_viewModel"),
    //                                 IdentifierName("viewModel"))),
    //                         ExpressionStatement(
    //                             AssignmentExpression(
    //                                 SyntaxKind.SimpleAssignmentExpression,
    //                                 IdentifierName("_visualElement"),
    //                                 IdentifierName("visualElement"))))),
    //             MethodDeclaration(
    //                     PredefinedType(
    //                         Token(SyntaxKind.VoidKeyword)),
    //                     Identifier("UpdateValues"))
    //                 .WithModifiers(
    //                     TokenList(
    //                         Token(SyntaxKind.PublicKeyword)))
    //                 .WithBody(
    //                     Block(GetUpdateValueMethodBody(bindableElementCapture.Properties)))
    //         })).NormalizeWhitespace();
    }
    // return ClassDeclaration("LabelBindings")
        //     .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
        //     .WithBaseList(BaseList(
        //         SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(IdentifierName("IVisualElementBindings")))))
        //     .WithMembers(List(new MemberDeclarationSyntax[]
        //     {
        //         FieldDeclaration(VariableDeclaration(IdentifierName("MainMenuViewModel"))
        //                 .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_viewModel")))))
        //             .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
        //         FieldDeclaration(VariableDeclaration(IdentifierName("BindableLabel"))
        //                 .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_visualElement")))))
        //             .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
        //         ConstructorDeclaration(Identifier("LabelBindings"))
        //             .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
        //             .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
        //             {
        //                 Parameter(Identifier("viewModel")).WithType(IdentifierName("MainMenuViewModel")),
        //                 Token(SyntaxKind.CommaToken),
        //                 Parameter(Identifier("visualElement")).WithType(IdentifierName("BindableLabel"))
        //             })))
        //             .WithBody(Block(
        //                 ExpressionStatement(
        //                     AssignmentExpression(
        //                         SyntaxKind.SimpleAssignmentExpression,
        //                         IdentifierName("_viewModel"),
        //                         IdentifierName("viewModel"))),
        //                 ExpressionStatement(
        //                     AssignmentExpression(
        //                         SyntaxKind.SimpleAssignmentExpression,
        //                         IdentifierName("_visualElement"),
        //                         IdentifierName("visualElement"))))),
        //         MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("UpdateValues"))
        //             .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
        //             .WithBody(Block(SingletonList<StatementSyntax>(
        //                 ExpressionStatement(
        //                     InvocationExpression(IdentifierName("UpdateStrValue")))))),
        //         MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("UpdateStrValue"))
        //             .WithAttributeLists(SingletonList(
        //                 AttributeList(SingletonSeparatedList(Attribute(IdentifierName("MethodImpl"))
        //                     .WithArgumentList(
        //                         AttributeArgumentList(SingletonSeparatedList(
        //                             AttributeArgument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
        //                                 IdentifierName("MethodImplOptions"),
        //                                 IdentifierName("AggressiveInlining"))))))))))
        //             .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
        //             .WithBody(Block(
        //                 SingletonList<StatementSyntax>(
        //                     IfStatement(
        //                         BinaryExpression(
        //                             SyntaxKind.NotEqualsExpression,
        //                             MemberAccessExpression(
        //                                 SyntaxKind.SimpleMemberAccessExpression,
        //                                 IdentifierName("_visualElement"),
        //                                 IdentifierName("text")),
        //                             MemberAccessExpression(
        //                                 SyntaxKind.SimpleMemberAccessExpression,
        //                                 IdentifierName("_viewModel"),
        //                                 IdentifierName("StrValue"))),
        //                         Block(
        //                             SingletonList<StatementSyntax>(
        //                                 ExpressionStatement(
        //                                     AssignmentExpression(
        //                                         SyntaxKind
        //                                             .SimpleAssignmentExpression,
        //                                         MemberAccessExpression(
        //                                             SyntaxKind
        //                                                 .SimpleMemberAccessExpression,
        //                                             IdentifierName(
        //                                                 "_visualElement"),
        //                                             IdentifierName("text")),
        //                                         MemberAccessExpression(
        //                                             SyntaxKind
        //                                                 .SimpleMemberAccessExpression,
        //                                             IdentifierName("_viewModel"),
        //                                             IdentifierName(
        //                                                 "StrValue"))))))))))
        //     })).NormalizeWhitespace();
    //}

    private IEnumerable<StatementSyntax> GetUpdateValueMethodBody(List<KeyValuePair<string, PropertyDeclarationSyntax>> properties)
    {
        var updateValueExpressions = new List<StatementSyntax>();

        foreach (var property in properties)
        {
            var leftExpression = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("_visualElement"), IdentifierName(property.Key));

            var rightExpression = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("_viewModel"), IdentifierName(property.Value.Identifier));

            updateValueExpressions.Add(IfStatement(
                BinaryExpression(SyntaxKind.NotEqualsExpression, leftExpression, rightExpression),
                Block(SingletonList<StatementSyntax>(ExpressionStatement(
                    AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, leftExpression, rightExpression))))));
        }

        return updateValueExpressions;

        //
        // return new StatementSyntax[]
        // {
        //     IfStatement(
        //         BinaryExpression(
        //             SyntaxKind.NotEqualsExpression,
        //             MemberAccessExpression(
        //                 SyntaxKind.SimpleMemberAccessExpression,
        //                 IdentifierName("_visualElement"),
        //                 IdentifierName("text")),
        //             MemberAccessExpression(
        //                 SyntaxKind.SimpleMemberAccessExpression,
        //                 IdentifierName("_viewModel"),
        //                 IdentifierName("StrValue"))),
        //         Block(
        //             SingletonList<StatementSyntax>(
        //                 ExpressionStatement(
        //                     AssignmentExpression(
        //                         SyntaxKind.SimpleAssignmentExpression,
        //                         MemberAccessExpression(
        //                             SyntaxKind.SimpleMemberAccessExpression,
        //                             IdentifierName("_visualElement"),
        //                             IdentifierName("text")),
        //                         MemberAccessExpression(
        //                             SyntaxKind.SimpleMemberAccessExpression,
        //                             IdentifierName("_viewModel"),
        //                             IdentifierName("StrValue"))))))),
        //     IfStatement(
        //         BinaryExpression(
        //             SyntaxKind.NotEqualsExpression,
        //             MemberAccessExpression(
        //                 SyntaxKind.SimpleMemberAccessExpression,
        //                 IdentifierName("_visualElement"),
        //                 IdentifierName("text")),
        //             MemberAccessExpression(
        //                 SyntaxKind.SimpleMemberAccessExpression,
        //                 IdentifierName("_viewModel"),
        //                 IdentifierName("StrValue"))),
        //         Block(
        //             SingletonList<StatementSyntax>(
        //                 ExpressionStatement(
        //                     AssignmentExpression(
        //                         SyntaxKind.SimpleAssignmentExpression,
        //                         MemberAccessExpression(
        //                             SyntaxKind.SimpleMemberAccessExpression,
        //                             IdentifierName("_visualElement"),
        //                             IdentifierName("text")),
        //                         MemberAccessExpression(
        //                             SyntaxKind.SimpleMemberAccessExpression,
        //                             IdentifierName("_viewModel"),
        //                             IdentifierName("StrValue")))))))
        // };
    }

    // private MethodDeclarationSyntax CreateViewClass
    
    private string GenerateView(ITypeSymbol typeSymbol, GeneratorExecutionContext context, ViewCapture view)
    {
        var usingDirectives = view.Class.SyntaxTree.GetRoot().DescendantNodes()
            .OfType<UsingDirectiveSyntax>();
        var usingDirectivesAsText = string.Join("\r\n", usingDirectives);
            
        return $@"using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.BindableVisualElements;

{usingDirectivesAsText}

namespace {typeSymbol.ContainingNamespace}
{{
    public partial class {typeSymbol.Name}
    {{
// {view.AssetPath}
// {view.Class.Identifier.Text}
// {view.ViewModelIdentifier}
// {view.Class.GetParent<NamespaceDeclarationSyntax>().Name}
        {GenerateBody(typeSymbol)}
    }}
}}";
    }

    private IEnumerable<BindableElementInfo> GetBindableElements(string assetFullPath)
    {
        var result = new List<BindableElementInfo>();

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
            
            var bindableElementInfo = new BindableElementInfo(node.Name.Split('.').Last());

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

    private string GenerateBody(ITypeSymbol typeSymbol)
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

    // private class SyntaxReceiver : ISyntaxContextReceiver
    // {
    //     public List<ITypeSymbol> Classes { get; } = new List<ITypeSymbol>();
    //
    //     /// <summary>
    //     /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
    //     /// </summary>
    //     public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    //     {
    //         // any field with at least one attribute is a candidate for property generation
    //         if (context.Node is ClassDeclarationSyntax classDeclarationSyntax &&
    //             classDeclarationSyntax.AttributeLists.Count > 0)
    //         {
    //             // Classes.Add(classDeclarationSyntax.dec.Select(x => semanticModel.GetDeclaredSymbol(x))
    //             //     .OfType<ITypeSymbol>());
    //             // if (classDeclarationSyntax.AttributeLists.Any(ad => ad.))
    //             // foreach (var variable in classDeclarationSyntax.Declaration.Variables)
    //             // {
    //             //     // Get the symbol being declared by the field, and keep it if its annotated
    //             //     ITypeSymbol fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
    //             //     if (fieldSymbol.GetAttributes().Any(ad => ad.AttributeClass.ToDisplayString() == "AutoNotify.AutoNotifyAttribute"))
    //             //     {
    //             //         Fields.Add(fieldSymbol);
    //             //     }
    //             // }
    //         }
    //     }
    // }
}