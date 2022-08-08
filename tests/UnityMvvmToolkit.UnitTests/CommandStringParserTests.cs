using FluentAssertions;
using UnityMvvmToolkit.Core.Internal.StringParsers;

namespace UnityMvvmToolkit.UnitTests;

public class CommandStringParserTests
{
    [Theory]
    [InlineData("ClickCommand", "ClickCommand", "", "")]
    [InlineData("ClickCommand, 55", "ClickCommand", "55", "")]
    [InlineData("ClickCommand, 55, ParameterToIntConverter", "ClickCommand", "55", "ParameterToIntConverter")]
    [InlineData("ClickCommand, Parameter={55}, Converter={ParameterConverter}", "ClickCommand", "55", "ParameterConverter")]
    [InlineData("ClickCommand, Converter={ParameterConverter}, Parameter={55}", "ClickCommand", "55", "ParameterConverter")]
    public void GetCommandData_ShouldParseString_WhenParametersAreValid(string propertyStringData, string propertyName,
        string parameterValue, string parameterConverterName)
    {
        // Arrange
        var commandParser = new CommandStringParser();

        // Act
        var result = commandParser.GetCommandData(propertyStringData.AsMemory());

        // Assert
        result.PropertyName.IsEmpty.Should().Be(string.IsNullOrEmpty(propertyName));
        result.ParameterValue.IsEmpty.Should().Be(string.IsNullOrEmpty(parameterValue));
        result.ParameterConverterName.IsEmpty.Should().Be(string.IsNullOrEmpty(parameterConverterName));
        
        result.PropertyName.ToString().Should().Be(propertyName);
        result.ParameterValue.ToString().Should().Be(parameterValue);
        result.ParameterConverterName.ToString().Should().Be(parameterConverterName);
    }
}