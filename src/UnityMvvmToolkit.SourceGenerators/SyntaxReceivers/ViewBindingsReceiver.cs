using Microsoft.CodeAnalysis;

namespace UnityMvvmToolkit.SourceGenerators.SyntaxReceivers;

public class ViewBindingsReceiver : ISyntaxReceiver
{
    public ViewsReceiver Views { get; } = new();
    public BindableElementsReceiver BindableElements { get; } = new();
    
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        BindableElements.OnVisitSyntaxNode(syntaxNode);
        Views.OnVisitSyntaxNode(syntaxNode);
    }
}