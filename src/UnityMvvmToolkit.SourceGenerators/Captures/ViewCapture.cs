using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnityMvvmToolkit.SourceGenerators.Captures;

public class ViewCapture
{
    public ViewCapture(string assetPath, string viewModelIdentifier, ClassDeclarationSyntax @class)
    {
        AssetPath = assetPath;
        ViewModelIdentifier = viewModelIdentifier;
        Class = @class;
    }

    public string AssetPath { get; }
    public string ViewModelIdentifier { get; }
    public ClassDeclarationSyntax Class { get; }
}