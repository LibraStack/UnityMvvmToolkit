using FluentAssertions;
using UnityMvvmToolkit.Core.Converters.ParameterValueConverters;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Test.Unit;

public class ParameterValueConverterTests
{
    private readonly ParameterToIntConverter _parameterToIntConverter;
    private readonly ParameterToStrConverter _parameterToStrConverter;
    private readonly ParameterToFloatConverter _parameterToFloatConverter;

    public ParameterValueConverterTests()
    {
        _parameterToIntConverter = new ParameterToIntConverter();
        _parameterToStrConverter = new ParameterToStrConverter();
        _parameterToFloatConverter = new ParameterToFloatConverter();
    }

    [Fact]
    public void ParameterValueConverter_ShouldCreateNewInstance()
    {
        // Arrange
        var targetType = typeof(int);

        IParameterValueConverter<int> parameterValueConverter = new ParameterToIntConverter();

        // Assert
        parameterValueConverter.Name.Should().Be(nameof(ParameterToIntConverter));
        parameterValueConverter.TargetType.Should().Be(targetType);
    }

    [Theory]
    [InlineData("69", 69)]
    [InlineData("-69", -69)]
    public void ParameterToIntConverter_ShouldConvertValue_WhenValueIsValid(string strValue, int intValue)
    {
        // Act
        var result = _parameterToIntConverter.Convert(strValue);

        // Assert
        result.Should().Be(intValue);
    }

    [Theory]
    [InlineData("69", 69)]
    [InlineData("-69", -69)]
    [InlineData("69.69", 69.69f)]
    [InlineData("69,69", 69.69f)]
    public void ParameterToFloatConverter_ShouldConvertValue_WhenValueIsValid(string strValue, float floatValue)
    {
        // Act
        var result = _parameterToFloatConverter.Convert(strValue);

        // Assert
        result.Should().Be(floatValue);
    }

    [Theory]
    [InlineData("69", "69")]
    [InlineData("Test", "Test")]
    public void ParameterToStrConverter_ShouldConvertValue_WhenValueIsValid(string strValue, string resultValue)
    {
        // Act
        var result = _parameterToStrConverter.Convert(strValue);

        // Assert
        result.Should().Be(resultValue);
    }
}