using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityMvvmToolkit.SourceGenerators.Captures;
using UnityMvvmToolkit.SourceGenerators.Extensions;

namespace UnityMvvmToolkit.SourceGenerators.SyntaxReceivers;

public class ViewsReceiver : ISyntaxReceiver
{
    private const string AttributeName = "VisualTreeAsset";

    private readonly List<ViewCapture> _captures = new();

    public IEnumerable<ViewCapture> Captures => _captures;

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not AttributeSyntax
            {
                Name: IdentifierNameSyntax { Identifier.Text: AttributeName }
            } attribute)
        {
            return;
        }

        if (attribute.ArgumentList?.Arguments.Single().Expression is not LiteralExpressionSyntax
            literalExpressionSyntax)
        {
            return;
        }
        
        var @class = attribute.GetParent<ClassDeclarationSyntax>();

        var genericNameSyntax = @class.BaseList?.Types.Single(type => type is SimpleBaseTypeSyntax).Type as GenericNameSyntax;
        if (genericNameSyntax?.TypeArgumentList.Arguments.Single() is not IdentifierNameSyntax identifierNameSyntax)
        {
            return;
        }

        var assetPath = literalExpressionSyntax.Token.ValueText;
        var viewModelIdentifier = identifierNameSyntax.Identifier.Text;
        
        _captures.Add(new ViewCapture(assetPath, viewModelIdentifier, @class));
    }
}