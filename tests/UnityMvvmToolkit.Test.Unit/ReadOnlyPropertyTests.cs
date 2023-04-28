using FluentAssertions;
using UnityMvvmToolkit.Core;

namespace UnityMvvmToolkit.Test.Unit;

public class ReadOnlyPropertyTests
{
    [Fact]
    public void ReadOnlyProperty_ShouldSetDefaultValue()
    {
        // Arrange
        const int value = 5;

        var readOnlyProperty = new ReadOnlyProperty<int>(value);

        // Assert
        readOnlyProperty.Value.GetType().Should().Be<int>();
        readOnlyProperty.Value.Should().Be(value);
    }

    [Fact]
    public void ReadOnlyProperty_ShouldImplicitlyConvertValue()
    {
        // Arrange
        const int value = 5;

        ReadOnlyProperty<int> readOnlyProperty = value;

        // Assert
        readOnlyProperty.Value.GetType().Should().Be<int>();
        readOnlyProperty.Value.Should().Be(value);
    }
}