using FluentAssertions;
using UnityMvvmToolkit.Core.Internal.StringParsers;
using UnityMvvmToolkit.Test.Unit.TestDataSets;

namespace UnityMvvmToolkit.Test.Unit;

public class CommandStringParserTests
{
    private readonly CommandStringParser _commandStringParser;

    public CommandStringParserTests()
    {
        _commandStringParser = new CommandStringParser();
    }

    [Theory]
    [ClassData(typeof(CommandValidBindingStringDataSet))]
    public void GetCommandData_ShouldReturnCommandBindingData_WhenBindingStringIsValid(string bindingString,
        string propertyName, string parameterValue, string parameterConverterName)
    {
        // Act
        var result = _commandStringParser.GetCommandData(0, bindingString.AsMemory());

        // Assert
        string.IsNullOrEmpty(result.PropertyName).Should().Be(string.IsNullOrEmpty(propertyName));
        string.IsNullOrEmpty(result.ParameterValue).Should().Be(string.IsNullOrEmpty(parameterValue));
        string.IsNullOrEmpty(result.ConverterName).Should().Be(string.IsNullOrEmpty(parameterConverterName));

        result.PropertyName.Should().Be(propertyName);
        result.ParameterValue.Should().Be(parameterValue);
        result.ConverterName.Should().Be(parameterConverterName);
    }

    [Theory]
    [ClassData(typeof(CommandNotValidBindingStringDataSet))]
    public void GetCommandData_ShouldThrow_WhenBindingStringIsNotValid(string bindingString)
    {
        // Arrange
        const int elementId = 69;

        // Assert
        _commandStringParser
            .Invoking(parser => parser.GetCommandData(elementId, bindingString.AsMemory()))
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage("lineData");
    }
}