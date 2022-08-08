using FluentAssertions;
using UnityMvvmToolkit.Common.Internal.StringParsers;

namespace UnityMvvmToolkit.UnitTests;

public class PropertyStringParserTests
{
    [Theory]
    [InlineData("Count", "Count", "")]
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
        result.PropertyName.IsEmpty.Should().Be(string.IsNullOrEmpty(propertyName));
        result.ConverterName.IsEmpty.Should().Be(string.IsNullOrEmpty(converterName));
        
        result.PropertyName.ToString().Should().Be(propertyName);
        result.ConverterName.ToString().Should().Be(converterName);
    }
}