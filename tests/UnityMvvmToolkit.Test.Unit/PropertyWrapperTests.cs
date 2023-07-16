using FluentAssertions;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.ObjectWrappers;

namespace UnityMvvmToolkit.Test.Unit;

public class PropertyWrapperTests
{
    private readonly IPropertyValueConverter<int, string> _intToStrConverter;

    public PropertyWrapperTests()
    {
        _intToStrConverter = new IntToStrConverter();
    }

    [Fact]
    public void PropertyWrapper_ShouldCreateNewPropertyWrapperInstance()
    {
        // Act
        var propertyWrapper = new PropertyConvertWrapper<int, string>(_intToStrConverter);

        // Assert
        propertyWrapper.ConverterId.Should().Be(-1);
    }

    [Fact]
    public void SetConverterId_ShouldSetButNotResetConverterId_WhenConverterIdIsNotSet()
    {
        // Arrange
        const int converterId = 5;

        var property = new Property<int>();
        var propertyWrapper = new PropertyConvertWrapper<int, string>(_intToStrConverter);

        // Act
        propertyWrapper
            .SetConverterId(converterId)
            .SetProperty(property);

        propertyWrapper.Reset();

        // Assert
        propertyWrapper.ConverterId.Should().Be(converterId);
    }

    [Fact]
    public void SetConverterId_ShouldThrow_WhenConverterIdAlreadySet()
    {
        // Arrange
        const int converterId = 5;

        var propertyWrapper = new PropertyConvertWrapper<int, string>(_intToStrConverter);

        // Act
        propertyWrapper.SetConverterId(converterId);

        // Assert
        propertyWrapper
            .Invoking(sut => sut.SetConverterId(converterId))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Can not change converter ID.");
    }

    [Fact]
    public void SetProperty_ShouldSetProperty_WhenPropertyIsNotSet()
    {
        // Arrange
        const int propertyValue = 55;

        var property = new Property<int>(propertyValue);
        var propertyWrapper = new PropertyConvertWrapper<int, string>(_intToStrConverter);

        // Act
        propertyWrapper.SetProperty(property);

        // Assert
        propertyWrapper.Value.Should().Be(_intToStrConverter.Convert(propertyValue));
        propertyWrapper
            .Invoking(sut => sut.Reset())
            .Should()
            .NotThrow();
    }

    [Fact]
    public void SetProperty_ShouldThrow_WhenPropertyAlreadySet()
    {
        // Arrange
        var property = new Property<int>();
        var propertyWrapper = new PropertyConvertWrapper<int, string>(_intToStrConverter);

        // Act
        propertyWrapper.SetProperty(property);

        // Assert
        propertyWrapper
            .Invoking(sut => sut.SetProperty(property))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"{nameof(propertyWrapper)} was not reset.");
    }

    [Fact]
    public void Reset_ShouldResetPropertyAndValue_WhenPropertyIsNotNull()
    {
        // Arrange
        const int propertyValue = 55;
        const int valueToSet = 25;

        var raisedCount = 0;
        string? expectedValue = default;

        var property = new Property<int>(propertyValue);
        var propertyWrapper = new PropertyConvertWrapper<int, string>(_intToStrConverter);

        propertyWrapper.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            expectedValue = newValue;
        };

        // Act
        propertyWrapper.SetProperty(property);
        propertyWrapper.Reset();

        property.Value = valueToSet;

        // Assert
        raisedCount.Should().Be(0);
        expectedValue.Should().Be(default);
        propertyWrapper.Value.Should().Be(default);
    }

    [Fact]
    public void Reset_ShouldThrow_WhenPropertyIsNull()
    {
        // Arrange
        var propertyWrapper = new PropertyConvertWrapper<int, string>(_intToStrConverter);

        // Assert
        propertyWrapper
            .Invoking(sut => sut.Reset())
            .Should()
            .Throw<NullReferenceException>();
    }

    [Fact]
    public void ForceSetValue_ShouldThrow()
    {
        // Arrange
        IProperty<string> propertyWrapper = new PropertyConvertWrapper<int, string>(_intToStrConverter);

        // Assert
        propertyWrapper
            .Invoking(sut => sut.ForceSetValue(default!))
            .Should()
            .Throw<NotImplementedException>();
    }

    [Fact]
    public void SetValue_ShouldRaisePropertyWrapperValueChangedEvent_WhenPropertyValueChanged()
    {
        // Arrange
        const int valueToSet = 55;

        var raisedCount = 0;
        string? expectedValue = default;

        var property = new Property<int>();
        var propertyWrapper = new PropertyConvertWrapper<int, string>(_intToStrConverter);

        propertyWrapper.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            expectedValue = newValue;
        };

        // Act
        propertyWrapper.SetProperty(property);

        property.Value = valueToSet;

        // Assert
        raisedCount.Should().Be(1);
        expectedValue.Should().Be(_intToStrConverter.Convert(valueToSet));
    }

    [Fact]
    public void SetValue_ShouldNotRaisePropertyWrapperValueChangedEvent_WhenPropertyValueNotChanged()
    {
        // Arrange
        const int valueToSet = 55;

        var raisedCount = 0;
        string? expectedValue = default;

        var property = new Property<int>(valueToSet);
        var propertyWrapper = new PropertyConvertWrapper<int, string>(_intToStrConverter);

        propertyWrapper.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            expectedValue = newValue;
        };

        // Act
        propertyWrapper.SetProperty(property);

        property.Value = valueToSet;

        // Assert
        raisedCount.Should().Be(0);
        expectedValue.Should().Be(default);
    }

    [Fact]
    public void SetValue_ShouldRaisePropertyValueChangedEvent_WhenPropertyWrapperValueChanged()
    {
        // Arrange
        const int valueToSet = 55;

        var raisedCount = 0;
        int expectedValue = default;

        var property = new Property<int>();
        var propertyWrapper = new PropertyConvertWrapper<int, string>(_intToStrConverter);

        property.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            expectedValue = newValue;
        };

        // Act
        propertyWrapper.SetProperty(property);
        propertyWrapper.Value = _intToStrConverter.Convert(valueToSet);

        // Assert
        raisedCount.Should().Be(1);
        expectedValue.Should().Be(valueToSet);
    }

    [Fact]
    public void SetValue_ShouldNotRaisePropertyValueChangedEvent_WhenPropertyWrapperValueNotChanged()
    {
        // Arrange
        const int valueToSet = 55;

        var raisedCount = 0;
        int expectedValue = default;

        var property = new Property<int>(valueToSet);
        var propertyWrapper = new PropertyConvertWrapper<int, string>(_intToStrConverter);

        property.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            expectedValue = newValue;
        };

        // Act
        propertyWrapper.SetProperty(property);
        propertyWrapper.Value = _intToStrConverter.Convert(valueToSet);

        // Assert
        raisedCount.Should().Be(0);
        expectedValue.Should().Be(default);
    }
}