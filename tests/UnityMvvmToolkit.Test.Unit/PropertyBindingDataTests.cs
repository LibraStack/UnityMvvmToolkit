using FluentAssertions;
using UnityMvvmToolkit.Core;

namespace UnityMvvmToolkit.Test.Unit;

public class PropertyBindingDataTests
{
    [Fact]
    public void SetValueByIndex_ShouldSetValueByIndex_WhenIndexIsValid()
    {
        // Arrange
        const string propertyName = "Count";
        const string converterName = "IntToStrConverter";

        var propertyBindingData = new PropertyBindingData();

        // Act
        propertyBindingData.SetValueByIndex(0, propertyName.AsMemory());
        propertyBindingData.SetValueByIndex(1, converterName.AsMemory());

        // Assert
        propertyBindingData.PropertyName.Should().Be(propertyName);
        propertyBindingData.ConverterName.Should().Be(converterName);
    }

    [Fact]
    public void SetValueByIndex_ShouldThrow_WhenIndexOutOfRange()
    {
        // Arrange
        var propertyBindingData = new PropertyBindingData();

        // Assert
        propertyBindingData
            .Invoking(sut => sut.SetValueByIndex(2, "".AsMemory()))
            .Should()
            .Throw<IndexOutOfRangeException>();
    }
}