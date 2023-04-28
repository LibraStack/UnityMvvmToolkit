using FluentAssertions;
using UnityMvvmToolkit.Core;

namespace UnityMvvmToolkit.Test.Unit;

public class CommandBindingDataTests
{
    [Fact]
    public void CommandBindingData_ShouldCreateNewInstance()
    {
        // Arrange
        const int elementId = 69;

        var commandBindingData = new CommandBindingData(elementId);

        // Assert
        commandBindingData.ElementId.Should().Be(elementId);
    }

    [Fact]
    public void SetValueByIndex_ShouldSetValueByIndex_WhenIndexIsValid()
    {
        // Arrange
        const int elementId = 69;
        const string propertyName = "Count";
        const string parameterValue = "69";
        const string converterName = "IntToStrConverter";

        var commandBindingData = new CommandBindingData(elementId);

        // Act
        commandBindingData.SetValueByIndex(0, propertyName.AsMemory());
        commandBindingData.SetValueByIndex(1, parameterValue.AsMemory());
        commandBindingData.SetValueByIndex(2, converterName.AsMemory());

        // Assert
        commandBindingData.PropertyName.Should().Be(propertyName);
        commandBindingData.ParameterValue.Should().Be(parameterValue);
        commandBindingData.ConverterName.Should().Be(converterName);
    }

    [Fact]
    public void SetValueByIndex_ShouldThrow_WhenIndexOutOfRange()
    {
        // Arrange
        const int elementId = 69;

        var commandBindingData = new CommandBindingData(elementId);

        // Assert
        commandBindingData
            .Invoking(sut => sut.SetValueByIndex(3, "".AsMemory()))
            .Should()
            .Throw<IndexOutOfRangeException>();
    }
}