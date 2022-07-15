using Microsoft.CodeAnalysis;

namespace UnityMvvmToolkit.SourceGenerators.Extensions;

public static class SyntaxNodeExtensions
{
    public static T GetParent<T>(this SyntaxNode syntaxNode)
    {
        var parent = syntaxNode.Parent;
            
        while (parent != null)
        {
            if (parent is T result)
            {
                return result;
            }

            parent = parent.Parent;
        }

        return default;
    }
}