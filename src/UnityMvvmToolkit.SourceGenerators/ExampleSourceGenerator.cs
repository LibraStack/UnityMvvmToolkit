using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using UnityMvvmToolkit.Common.Attributes;

namespace UnityMvvmToolkit.SourceGenerators
{
    [Generator]
    public class ExampleSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {

            var stringBuilder = new StringBuilder(
                @"
            using System;
            namespace ExampleSourceGenerated
            {
                public static class ExampleSourceGenerated
                {
                    public static string GetTestText() 
                    {
                        return ""This is from source generator ");

            stringBuilder.Append(nameof(BindToAttribute));

            stringBuilder.Append(
                @""";
                    }
    }
}
");

            context.AddSource("exampleSourceGenerator", SourceText.From(stringBuilder.ToString(), Encoding.UTF8));
        }
    }
}