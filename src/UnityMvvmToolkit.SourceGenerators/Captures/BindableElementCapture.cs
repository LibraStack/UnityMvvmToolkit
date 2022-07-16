using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnityMvvmToolkit.SourceGenerators.Captures;

public class BindableElementCapture
{
    public BindableElementCapture(ClassDeclarationSyntax @class)
    {
        Class = @class;
        Properties = new Dictionary<string, string>();
    }

    public string ClassIdentifier => Class.Identifier.Text;
    public ClassDeclarationSyntax Class { get; }
    public Dictionary<string, string> Properties { get; }
}