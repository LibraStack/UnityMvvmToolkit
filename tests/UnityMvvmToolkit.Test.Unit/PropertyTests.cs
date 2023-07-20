using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Test.Unit;

[SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
public class PropertyTests
{
    [Theory]
    [MemberData(nameof(PropertyDataSets), 5)]
    public void Property_ShouldSetDefaultValue<T>(IProperty<T> property, T defaultValue)
    {
        // Assert
        property.Value.Should().Be(defaultValue);
        property.Value!.GetType().Should().Be(typeof(T));
    }

    [Theory]
    [MemberData(nameof(PropertyWithSetValueDataSets), 0, 55)]
    public void Property_ShouldChangeValue_WhenValuesAraNotEqual<T>(IProperty<T> property, T _, T valueToSet)
    {
        // Act
        property.Value = valueToSet;

        // Assert
        property.Value.Should().Be(valueToSet);
    }

    [Theory]
    [MemberData(nameof(PropertyWithSetValueDataSets), 0, 69)]
    public void Property_ShouldRaiseValueChangedEvent_WhenPropertyChanged<T>(IProperty<T> property, T defaultValue,
        T valueToSet)
    {
        // Arrange
        var raisedCount = 0;
        var expectedValue = defaultValue;

        property.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            expectedValue = newValue;
        };

        // Act
        property.Value = valueToSet;

        // Assert
        raisedCount.Should().Be(1);
        expectedValue.Should().Be(valueToSet);
    }

    [Theory]
    [MemberData(nameof(PropertyWithSetValueDataSets), 25, 25)]
    public void Property_ShouldNotRaiseValueChangedEvent_WhenPropertyIsNotChanged<T>(IProperty<T> property,
        T defaultValue, T valueToSet)
    {
        // Arrange
        var raisedCount = 0;
        var expectedValue = defaultValue;

        property.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            expectedValue = newValue;
        };

        // Act
        property.Value = valueToSet;

        // Assert
        raisedCount.Should().Be(0);
        expectedValue.Should().Be(defaultValue);
    }

    [Theory]
    [MemberData(nameof(PropertyWithSetValueDataSets), 5, 15)]
    public void TrySetValue_ShouldSetValueAndReturnTrue_WhenValuesAreNotEqual<T>(IProperty<T> property, T _,
        T valueToSet)
    {
        // Assert
        property.TrySetValue(valueToSet).Should().Be(true);
        property.Value.Should().Be(valueToSet);
    }

    [Theory]
    [MemberData(nameof(PropertyWithSetValueDataSets), 5, 5)]
    public void TrySetValue_ShouldNotSetValueAndReturnFalse_WhenValuesAreNotEqual<T>(IProperty<T> property, T _,
        T valueToSet)
    {
        // Assert
        property.TrySetValue(valueToSet).Should().Be(false);
        property.Value.Should().Be(valueToSet);
    }

    [Theory]
    [MemberData(nameof(PropertyWithSetValueDataSets), 0, 11)]
    public void TrySetValue_ShouldRaiseValueChangedEvent_WhenPropertyChanged<T>(IProperty<T> property, T defaultValue,
        T valueToSet)
    {
        // Arrange
        var raisedCount = 0;
        var expectedValue = defaultValue;

        property.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            expectedValue = newValue;
        };

        // Assert
        property.TrySetValue(valueToSet).Should().Be(true);
        raisedCount.Should().Be(1);
        expectedValue.Should().Be(valueToSet);
    }

    [Theory]
    [MemberData(nameof(PropertyWithSetValueDataSets), 11, 11)]
    public void TrySetValue_ShouldNotRaiseValueChangedEvent_WhenPropertyIsNotChanged<T>(IProperty<T> property,
        T defaultValue, T valueToSet)
    {
        // Arrange
        var raisedCount = 0;
        var expectedValue = defaultValue;

        property.ValueChanged += (_, newValue) =>
        {
            raisedCount++;
            expectedValue = newValue;
        };

        // Assert
        property.TrySetValue(valueToSet).Should().Be(false);
        raisedCount.Should().Be(0);
        expectedValue.Should().Be(defaultValue);
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
        property.Value.Should().Be(testStr);
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

    private static IEnumerable<object[]> PropertyDataSets(int defaultValue)
    {
        yield return new object[] { new Property<int>(defaultValue), defaultValue };
    }

    private static IEnumerable<object[]> PropertyWithSetValueDataSets(int defaultValue, int valueToSet)
    {
        yield return new object[] { new Property<int>(defaultValue), defaultValue, valueToSet };
    }
}