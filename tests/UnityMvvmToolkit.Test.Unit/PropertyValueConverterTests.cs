using FluentAssertions;
using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Test.Unit;

public class PropertyValueConverterTests
{
    private readonly IntToStrConverter _intToStrConverter;
    private readonly FloatToStrConverter _floatToStrConverter;

    public PropertyValueConverterTests()
    {
        _intToStrConverter = new IntToStrConverter();
        _floatToStrConverter = new FloatToStrConverter();
    }

    [Fact]
    public void ParameterValueConverter_ShouldCreateNewInstance()
    {
        // Arrange
        var sourceType = typeof(int);
        var targetType = typeof(string);

        IPropertyValueConverter<int, string> parameterValueConverter = new IntToStrConverter();

        // Assert
        parameterValueConverter.Name.Should().Be(nameof(IntToStrConverter));
        parameterValueConverter.SourceType.Should().Be(sourceType);
        parameterValueConverter.TargetType.Should().Be(targetType);
    }
    
    [Theory]
    [InlineData(69, "69")]
    [InlineData(-69, "-69")]
    public void IntToStrConverter_ShouldConvertValue_WhenValueIsValid(int intValue, string strValue)
    {
        // Act
        var result = _intToStrConverter.Convert(intValue);

        // Assert
        result.Should().Be(strValue);
    }

    [Theory]
    [InlineData("69", 69)]
    [InlineData("-69", -69)]
    public void IntToStrConverter_ShouldConvertBackValue_WhenValueIsValid(string strValue, int intValue)
    {
        // Act
        var result = _intToStrConverter.ConvertBack(strValue);

        // Assert
        result.Should().Be(intValue);
    }

    [Theory]
    [InlineData(69, "69")]
    [InlineData(-69, "-69")]
    [InlineData(69.69f, "69.69", "69,69")]
    public void ParameterToFloatConverter_ShouldConvertValue_WhenValueIsValid(float floatValue,
        params string[] strValue)
    {
        // Act
        var result = _floatToStrConverter.Convert(floatValue);

        // Assert
        result.Should().BeOneOf(strValue);
    }

    [Theory]
    [InlineData("69", 69)]
    [InlineData("-69", -69)]
    [InlineData("69.69", 69.69f)]
    [InlineData("69,69", 69.69f)]
    public void ParameterToFloatConverter_ShouldConvertBackValue_WhenValueIsValid(string strValue, float floatValue)
    {
        // Act
        var result = _floatToStrConverter.ConvertBack(strValue);

        // Assert
        result.Should().Be(floatValue);
    }
}