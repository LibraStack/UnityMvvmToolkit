using FluentAssertions;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Test.Unit;

public class PropertyTests
{
    [Fact]
    public void Property_ShouldSetDefaultValue()
    {
        // Arrange
        const int value = 5;

        var property = new Property<int>(value);

        // Assert
        property.Value.GetType().Should().Be<int>();
        property.Value.Should().Be(value);
    }

    [Fact]
    public void Property_ShouldChangeValue_WhenValuesIsNotEqual()
    {
        // Arrange
        const int value = 5;

        var property = new Property<int>();

        // Act
        property.Value = value;

        // Assert
        property.Value.GetType().Should().Be<int>();
        property.Value.Should().Be(value);
    }

    [Fact]
    public void Property_ShouldRaiseValueChangedEvent_WhenPropertyChanged()
    {
        // Arrange
        const string testStr = "Test";

        var raisedCount = 0;
        string? newStrValue = default;

        var property = new Property<string>();
        property.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            newStrValue = newValue;
        };

        // Act
        property.Value = testStr;

        // Assert
        raisedCount.Should().Be(1);
        newStrValue.Should().Be(testStr);
    }

    [Fact]
    public void Property_ShouldNotRaiseValueChangedEvent_WhenPropertyIsNotChanged()
    {
        // Arrange
        const string testStr = "Test";

        var raisedCount = 0;
        string? newStrValue = default;

        var property = new Property<string>(testStr);
        property.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            newStrValue = newValue;
        };

        // Act
        property.Value = testStr;

        // Assert
        raisedCount.Should().Be(0);
        newStrValue.Should().Be(default);
    }

    [Fact]
    public void TrySetValue_ShouldSetValueAndReturnTrue_WhenValuesAreNotEqual()
    {
        // Arrange
        const string initStr = "Init";
        const string testStr = "Test";

        bool setResult;

        var property = new Property<string>(initStr);

        // Act
        setResult = property.TrySetValue(testStr);

        // Assert
        setResult.Should().Be(true);
        property.Value.Should().Be(testStr);
    }

    [Fact]
    public void TrySetValue_ShouldNotSetValueAndReturnFalse_WhenValuesAreNotEqual()
    {
        // Arrange
        const string initStr = "Init";

        bool setResult;

        var property = new Property<string>(initStr);

        // Act
        setResult = property.TrySetValue(initStr);

        // Assert
        setResult.Should().Be(false);
        property.Value.Should().Be(initStr);
    }

    [Fact]
    public void TrySetValue_ShouldRaiseValueChangedEvent_WhenPropertyChanged()
    {
        // Arrange
        const string testStr = "Test";

        bool setResult;
        var raisedCount = 0;
        string? newStrValue = default;

        var property = new Property<string>();
        property.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            newStrValue = newValue;
        };

        // Act
        setResult = property.TrySetValue(testStr);

        // Assert
        setResult.Should().Be(true);
        raisedCount.Should().Be(1);
        newStrValue.Should().Be(testStr);
    }

    [Fact]
    public void TrySetValue_ShouldNotRaiseValueChangedEvent_WhenPropertyIsNotChanged()
    {
        // Arrange
        const string testStr = "Test";

        bool setResult;
        var raisedCount = 0;
        string? newStrValue = default;

        var property = new Property<string>(testStr);
        property.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            newStrValue = newValue;
        };

        // Act
        setResult = property.TrySetValue(testStr);

        // Assert
        setResult.Should().Be(false);
        raisedCount.Should().Be(0);
        newStrValue.Should().Be(default);
    }

    [Fact]
    public void ForceSetValue_ShouldSetValue_EvenIfValuesAreEqual()
    {
        // Arrange
        const string testStr = "Test";

        var raisedCount = 0;
        string? newStrValue = default;

        IProperty<string> property = new Property<string>(testStr);
        property.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            newStrValue = newValue;
        };

        // Act
        property.ForceSetValue(testStr);

        // Assert
        raisedCount.Should().Be(1);
        newStrValue.Should().Be(testStr);
    }

    [Fact]
    public void Property_ShouldImplicitlyConvertValue()
    {
        // Arrange
        const int value = 5;

        Property<int> property = value;

        // Assert
        property.Value.GetType().Should().Be<int>();
        property.Value.Should().Be(value);
    }
}