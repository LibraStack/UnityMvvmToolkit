using FluentAssertions;
using UnityMvvmToolkit.Core.Internal.StringParsers;

namespace UnityMvvmToolkit.UnitTests;

public class CommandStringParserTests
{
    [Theory]
    [InlineData("ClickCommand", "ClickCommand", null, null)]
    [InlineData("ClickCommand, 55", "ClickCommand", "55", null)]
    [InlineData("ClickCommand, 55, ParameterToIntConverter", "ClickCommand", "55", "ParameterToIntConverter")]
    [InlineData("ClickCommand, Parameter={55}, Converter={ParameterConverter}", "ClickCommand", "55", "ParameterConverter")]
    [InlineData("ClickCommand, Converter={ParameterConverter}, Parameter={55}", "ClickCommand", "55", "ParameterConverter")]
    public void GetCommandData_ShouldParseString_WhenParametersAreValid(string propertyStringData, string propertyName,
        string parameterValue, string parameterConverterName)
    {
        // Arrange
        var commandParser = new CommandStringParser();

        // Act
        var result = commandParser.GetCommandData(0, propertyStringData.AsMemory());

        // Assert
        string.IsNullOrEmpty(result.PropertyName).Should().Be(string.IsNullOrEmpty(propertyName));
        string.IsNullOrEmpty(result.ParameterValue).Should().Be(string.IsNullOrEmpty(parameterValue));
        string.IsNullOrEmpty(result.ParameterConverterName).Should().Be(string.IsNullOrEmpty(parameterConverterName));

        result.PropertyName.Should().Be(propertyName);
        result.ParameterValue.Should().Be(parameterValue);
        result.ParameterConverterName.Should().Be(parameterConverterName);
    }
}