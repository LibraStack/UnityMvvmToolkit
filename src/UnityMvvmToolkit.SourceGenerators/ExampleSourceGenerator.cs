using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

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

            var sourceBuilder = new StringBuilder(
                @"
            using System;
            namespace ExampleSourceGenerated
            {
                public static class ExampleSourceGenerated
                {
                    public static string GetTestText() 
                    {
                        return ""This is from source generator ");

            sourceBuilder.Append(System.DateTime.Now.ToString(CultureInfo.CurrentCulture));

            sourceBuilder.Append(
                @""";
                    }
    }
}
");

            context.AddSource("exampleSourceGenerator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}