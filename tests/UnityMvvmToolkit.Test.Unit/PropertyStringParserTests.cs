using FluentAssertions;
using UnityMvvmToolkit.Core.Internal.StringParsers;
using UnityMvvmToolkit.Test.Unit.TestDataSets;

namespace UnityMvvmToolkit.Test.Unit;

public class PropertyStringParserTests
{
    private readonly PropertyStringParser _propertyStringParser;

    public PropertyStringParserTests()
    {
        _propertyStringParser = new PropertyStringParser();
    }

    [Theory]
    [ClassData(typeof(PropertyValidBindingStringDataSet))]
    public void GetPropertyData_ShouldReturnPropertyBindingData_WhenBindingStringIsValid(string bindingString,
        string propertyName, string converterName)
    {
        // Act
        var result = _propertyStringParser.GetPropertyData(bindingString.AsMemory());

        // Assert
        string.IsNullOrEmpty(result.PropertyName).Should().Be(string.IsNullOrEmpty(propertyName));
        string.IsNullOrEmpty(result.ConverterName).Should().Be(string.IsNullOrEmpty(converterName));

        result.PropertyName.Should().Be(propertyName);
        result.ConverterName.Should().Be(converterName);
    }

    [Theory]
    [ClassData(typeof(PropertyNotValidBindingStringDataSet))]
    public void GetPropertyData_ShouldThrow_WhenBindingStringIsNotValid(string bindingString)
    {
        // Assert
        _propertyStringParser
            .Invoking(parser => parser.GetPropertyData(bindingString.AsMemory()))
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage("lineData");
    }
}