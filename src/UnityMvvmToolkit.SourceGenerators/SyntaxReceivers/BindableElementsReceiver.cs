using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.SourceGenerators.Captures;
using UnityMvvmToolkit.SourceGenerators.Extensions;

namespace UnityMvvmToolkit.SourceGenerators.SyntaxReceivers;

public class BindableElementsReceiver : ISyntaxReceiver
{
    private const string AttributeName = "BindTo";
    private const string InterfaceName = nameof(IBindableVisualElement);

    private readonly Dictionary<string, BindableElementCapture> _captures = new();

    public IReadOnlyDictionary<string, BindableElementCapture> Captures => _captures;

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not AttributeSyntax
            {
                Name: IdentifierNameSyntax { Identifier.Text: AttributeName }
            } attribute)
        {
            return;
        }

        var bindToPath = GetAttributeArgumentValue(attribute);
        if (string.IsNullOrWhiteSpace(bindToPath))
        {
            return;
        }
        
        var property = attribute.GetParent<PropertyDeclarationSyntax>();
        var @class = property.GetParent<ClassDeclarationSyntax>();

        if (IsImplementInterface(@class, InterfaceName) == false)
        {
            return;
        }

        if (_captures.TryGetValue(@class.Identifier.Text, out var bindableElement) == false)
        {
            bindableElement = new BindableElementCapture(@class);
            _captures.Add(@class.Identifier.Text, bindableElement);
        }
        
        bindableElement.Properties.Add(new KeyValuePair<string, PropertyDeclarationSyntax>(bindToPath, property));
    }

    private string GetAttributeArgumentValue(AttributeSyntax attribute)
    {
        return attribute.ArgumentList?.Arguments.Single().Expression switch
        {
            LiteralExpressionSyntax literal => literal.Token.ValueText,
            InvocationExpressionSyntax invocation => invocation.ArgumentList.Arguments.Single().Expression.GetText().ToString(),
            _ => null
        };
    }
    
    private bool IsImplementInterface(ClassDeclarationSyntax @class, string @interface)
    {
        return @class.BaseList == null || 
               @class.BaseList.Types.Select(typeSyntax => typeSyntax.Type.GetText().ToString() == @interface).Any();
    }
}