using FluentAssertions;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Converters.ParameterValueConverters;
using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UnitTests.TestBindingContext;

namespace UnityMvvmToolkit.UnitTests;

public class BindingContextObjectProviderTests
{
    [Fact]
    public void RentProperty_ShouldReturnProperty_WhenDataIsValid()
    {
        // Arrange
        const int countValue = 69;
        const string titleValue = "TestTitle";

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext(titleValue)
        {
            Count = countValue,
        };

        IProperty<int> countProperty;
        var countPropertyBindingData = nameof(MyBindingContext.Count).ToPropertyBindingData();

        IReadOnlyProperty<string> titleProperty;
        var titlePropertyBindingData = nameof(MyBindingContext.Title).ToPropertyBindingData();

        // Act
        countProperty = objectProvider.RentProperty<int>(bindingContext, countPropertyBindingData);
        titleProperty = objectProvider.RentReadOnlyProperty<string>(bindingContext, titlePropertyBindingData);

        // Assert
        countProperty
            .Should().NotBeNull()
            .And
            .BeAssignableTo<IProperty<int>>()
            .And
            .BeAssignableTo<IReadOnlyProperty<int>>();

        titleProperty
            .Should().NotBeNull()
            .And
            .BeAssignableTo<IReadOnlyProperty<string>>()
            .And
            .NotBeAssignableTo<IProperty<string>>();

        countProperty.Value.Should().Be(countValue);
        titleProperty.Value.Should().Be(titleValue);
    }

    [Fact]
    public void RentPropertyWithConverter_ShouldReturnProperty_WhenDataIsValid()
    {
        // Arrange
        const int countValue = 69;

        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new IntToStrConverter()
        });

        var bindingContext = new MyBindingContext
        {
            Count = countValue,
        };

        IProperty<string> countProperty;
        var countPropertyBindingData = nameof(MyBindingContext.Count).ToPropertyBindingData();

        // Act
        countProperty = objectProvider.RentProperty<string>(bindingContext, countPropertyBindingData);

        // Assert
        countProperty
            .Should()
            .NotBeNull()
            .And
            .BeAssignableTo<IProperty<string>>()
            .And
            .BeAssignableTo<IReadOnlyProperty<string>>();

        countProperty.Value.Should().Be(countValue.ToString());
    }

    [Fact]
    public void GetCommand_ShouldReturnCommand_WhenDataIsValid()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        ICommand incrementCommand;
        ICommand decrementCommand;

        // Act
        incrementCommand =
            objectProvider.GetCommand<ICommand>(bindingContext, nameof(MyBindingContext.IncrementCommand));

        decrementCommand =
            objectProvider.GetCommand<ICommand>(bindingContext, nameof(MyBindingContext.DecrementCommand));

        // Assert
        incrementCommand.Should().NotBeNull().And.BeAssignableTo<IBaseCommand>();
        decrementCommand.Should().NotBeNull().And.BeAssignableTo<IBaseCommand>();
    }

    [Fact]
    public void RentCommandWrapper_ShouldReturnCommand_WhenDataIsValid()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new ParameterToIntConverter()
        });

        var bindingContext = new MyBindingContext();

        IBaseCommand setValueCommand;
        var setValueCommandBindingData = $"{nameof(MyBindingContext.SetValueCommand)}, 5".ToCommandBindingData(0);

        // Act
        setValueCommand = objectProvider.RentCommandWrapper(bindingContext, setValueCommandBindingData);

        // Assert
        setValueCommand.Should().NotBeNull();
    }

    [Fact]
    public void RentPropertyWithConverter_ShouldThrow_WhenConverterIsNotSet()
    {
        // Arrange
        const int countValue = 69;

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext
        {
            Count = countValue,
        };

        var countPropertyBindingData = nameof(MyBindingContext.Count).ToPropertyBindingData();

        // Assert
        objectProvider
            .Invoking(objProvider => objProvider.RentProperty<string>(bindingContext, countPropertyBindingData))
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage($"Property value converter from '{typeof(int)}' to '{typeof(string)}' not found.");
    }

    [Fact]
    public void RentCommandWrapper_ShouldThrow_WhenConverterIsNotSet()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        var setValueCommandBindingData = $"{nameof(MyBindingContext.SetValueCommand)}, 5".ToCommandBindingData(0);

        // Assert
        objectProvider
            .Invoking(objProvider => objProvider.RentCommandWrapper(bindingContext, setValueCommandBindingData))
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage($"Parameter value converter to '{typeof(int)}' not found.");
    }
}