using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using UnityMvvmToolkit.Core.Converters.ParameterValueConverters;
using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Helpers;
using UnityMvvmToolkit.Core.Internal.ObjectHandlers;

namespace UnityMvvmToolkit.Test.Unit;

[SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
public class ValueConverterHandlerTests
{
    [Theory]
    [MemberData(nameof(PropertyConverterDataSets))]
    [MemberData(nameof(ParameterConverterDataSets))]
    public void TryGetValueConverterById_ShouldReturnValueConverter_WhenConverterIsRegistered(IValueConverter converter,
        int converterId)
    {
        // Arrange
        var valueConverterHandler = new ValueConverterHandler(new[] { converter });

        // Act
        var isSuccess = valueConverterHandler.TryGetValueConverterById(converterId, out var result);

        // Assert
        isSuccess.Should().Be(true);
        result.Should().BeEquivalentTo(converter);
    }

    [Theory]
    [MemberData(nameof(PropertyConverterDataSets))]
    [MemberData(nameof(ParameterConverterDataSets))]
    public void TryGetValueConverterByType_ShouldReturnPropertyValueConverter_WhenValueConverterIsRegistered(
        IValueConverter converter, int _)
    {
        // Arrange
        var valueConverterHandler = new ValueConverterHandler(new[] { converter });

        // Act
        var isSuccess = valueConverterHandler.TryGetValueConverterByType(converter.GetType(), out var result);

        // Assert
        isSuccess.Should().Be(true);
        result.Should().BeEquivalentTo(converter);
    }

    [Fact]
    public void TryGetValueConverterById_ShouldReturnParameterToStrConverter_WhenConverterIsNotRegistered()
    {
        // Arrange
        var converter = new ParameterToStrConverter();
        var converterId = HashCodeHelper.GetParameterConverterHashCode(converter);
        var valueConverterHandler = new ValueConverterHandler(Array.Empty<IValueConverter>());

        // Act
        var isSuccess = valueConverterHandler.TryGetValueConverterById(converterId, out var result);

        // Assert
        isSuccess.Should().Be(true);
        result.Should().BeEquivalentTo(converter);
    }

    [Fact]
    public void TryGetValueConverterByType_ShouldReturnParameterToStrConverter_WhenConverterIsNotRegistered()
    {
        // Arrange
        var converter = new ParameterToStrConverter();
        var valueConverterHandler = new ValueConverterHandler(Array.Empty<IValueConverter>());

        // Act
        var isSuccess = valueConverterHandler.TryGetValueConverterByType(converter.GetType(), out var result);

        // Assert
        isSuccess.Should().Be(true);
        result.Should().BeEquivalentTo(converter);
    }

    [Fact]
    public void TryGetValueConverterById_ShouldNotReturnPropertyValueConverter_WhenValueConverterIsNotRegistered()
    {
        // Arrange
        var converterId = HashCodeHelper.GetPropertyConverterHashCode(new IntToStrConverter());
        var valueConverterHandler = new ValueConverterHandler(Array.Empty<IValueConverter>());

        // Act
        var isSuccess = valueConverterHandler.TryGetValueConverterById(converterId, out var result);

        // Assert
        isSuccess.Should().Be(false);
        result.Should().Be(default);
    }

    [Fact]
    public void TryGetValueConverterByType_ShouldNotReturnValueConverter_WhenValueConverterIsNotRegistered()
    {
        // Arrange
        var valueConverterHandler = new ValueConverterHandler(Array.Empty<IValueConverter>());

        // Act
        var isSuccess = valueConverterHandler.TryGetValueConverterByType(typeof(IntToStrConverter), out var result);

        // Assert
        isSuccess.Should().Be(false);
        result.Should().Be(default);
    }

    [Fact]
    public void Dispose_ShouldClearCollection()
    {
        // Arrange
        var intToStrConverter = new IntToStrConverter();

        var valueConverterHandler = new ValueConverterHandler(new IValueConverter[]
        {
            intToStrConverter
        });

        // Act
        valueConverterHandler.Dispose();

        var isSuccess = valueConverterHandler.TryGetValueConverterByType(typeof(IntToStrConverter), out var result);

        // Assert
        isSuccess.Should().Be(false);
        result.Should().Be(default);
    }

    private static IEnumerable<object[]> PropertyConverterDataSets()
    {
        var propertyConverter = new IntToStrConverter();

        yield return new object[]
        {
            propertyConverter,
            propertyConverter.GetType().GetHashCode()
        };

        yield return new object[]
        {
            propertyConverter,
            HashCodeHelper.GetPropertyConverterHashCode(propertyConverter)
        };

        yield return new object[]
        {
            propertyConverter,
            HashCodeHelper.GetPropertyConverterHashCode(propertyConverter, propertyConverter.Name)
        };
    }

    private static IEnumerable<object[]> ParameterConverterDataSets()
    {
        var parameterConverter = new ParameterToIntConverter();

        yield return new object[]
        {
            parameterConverter,
            parameterConverter.GetType().GetHashCode()
        };

        yield return new object[]
        {
            parameterConverter,
            HashCodeHelper.GetParameterConverterHashCode(parameterConverter)
        };

        yield return new object[]
        {
            parameterConverter,
            HashCodeHelper.GetParameterConverterHashCode(parameterConverter, parameterConverter.Name)
        };
    }
}