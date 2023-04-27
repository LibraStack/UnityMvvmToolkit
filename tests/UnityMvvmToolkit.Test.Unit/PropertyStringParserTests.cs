using FluentAssertions;
using UnityMvvmToolkit.Core.Internal.StringParsers;

namespace UnityMvvmToolkit.Test.Unit;

public class PropertyStringParserTests
{
    [Theory]
    [InlineData("Count", "Count", null)]
    [InlineData("Count, IntToStrConverter", "Count", "IntToStrConverter")]
    [InlineData("Count, Converter={IntToStrConverter}", "Count", "IntToStrConverter")]
    public void GetPropertyData_ShouldParseString_WhenParametersAreValid(string propertyStringData, string propertyName,
        string converterName)
    {
        // Arrange
        var propertyParser = new PropertyStringParser();

        // Act
        var result = propertyParser.GetPropertyData(propertyStringData.AsMemory());

        // Assert
        string.IsNullOrEmpty(result.PropertyName).Should().Be(string.IsNullOrEmpty(propertyName));
        string.IsNullOrEmpty(result.ConverterName).Should().Be(string.IsNullOrEmpty(converterName));

        result.PropertyName.Should().Be(propertyName);
        result.ConverterName.Should().Be(converterName);
    }

    [Theory]
    [InlineData("Count,, IntToStrConverter")]
    [InlineData("Count, IntToStrConverter,,")]
    public void GetPropertyData_EnsureExceptionThrown(string propertyStringData)
    {
        // Arrange
        var propertyParser = new PropertyStringParser();

        // Assert
        propertyParser
            .Invoking(parser => parser.GetPropertyData(propertyStringData.AsMemory()))
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage("lineData");
    }
}