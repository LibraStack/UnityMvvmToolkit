using FluentAssertions;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Internal.Helpers;
using UnityMvvmToolkit.Test.Unit.TestDataSets;

namespace UnityMvvmToolkit.Test.Unit;

public class BindingStringHelperTests
{
    [Theory]
    [ClassData(typeof(PropertyValidBindingStringDataSet))]
    public void GetPropertyBindingData_ShouldReturnPropertyBindingData_WhenBindingStringIsValid(string bindingString,
        string propertyName, string converterName)
    {
        // Act
        var propertyBindingData = BindingStringHelper.GetPropertyBindingData(bindingString);
        var toPropertyBindingData = bindingString.ToPropertyBindingData();

        // Assert
        propertyBindingData.PropertyName.Should().Be(propertyName);
        propertyBindingData.ConverterName.Should().Be(converterName);

        propertyBindingData.Should().BeEquivalentTo(toPropertyBindingData);
    }

    [Theory]
    [ClassData(typeof(CommandValidBindingStringDataSet))]
    public void GetCommandBindingData_ShouldReturnCommandBindingData_WhenBindingStringIsValid(string bindingString,
        string propertyName, string parameterValue, string parameterConverterName)
    {
        // Arrange
        const int elementId = 69;

        // Act
        var commandBindingData = BindingStringHelper.GetCommandBindingData(elementId, bindingString);
        var toCommandBindingData = bindingString.ToCommandBindingData(elementId);

        // Assert
        commandBindingData.ElementId.Should().Be(elementId);
        commandBindingData.PropertyName.Should().Be(propertyName);
        commandBindingData.ParameterValue.Should().Be(parameterValue);
        commandBindingData.ConverterName.Should().Be(parameterConverterName);

        commandBindingData.Should().BeEquivalentTo(toCommandBindingData);
    }
}