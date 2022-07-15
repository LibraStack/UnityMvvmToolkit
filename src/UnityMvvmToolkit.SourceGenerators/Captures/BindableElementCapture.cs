using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnityMvvmToolkit.SourceGenerators.Captures;

public class BindableElementCapture
{
    public BindableElementCapture(ClassDeclarationSyntax @class)
    {
        Class = @class;
        Properties = new List<KeyValuePair<string, PropertyDeclarationSyntax>>();
    }

    public ClassDeclarationSyntax Class { get; }
    public List<KeyValuePair<string, PropertyDeclarationSyntax>> Properties { get; }
}