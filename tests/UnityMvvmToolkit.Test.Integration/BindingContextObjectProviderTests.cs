using System.Reflection;
using FluentAssertions;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Converters.ParameterValueConverters;
using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.ObjectHandlers;
using UnityMvvmToolkit.Core.Internal.ObjectWrappers;
using UnityMvvmToolkit.Test.Integration.TestBindingContext;
using UnityMvvmToolkit.Test.Unit.TestBindingContext;

namespace UnityMvvmToolkit.Test.Integration;

public class BindingContextObjectProviderTests
{
    [Fact]
    public void WarmupAssemblyViewModels_ShouldWarmupCallingAssemblyViewModels()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());

        // Act
        objectProvider.WarmupAssemblyViewModels();

        // Assert
        objectProvider
            .Invoking(sut => sut.WarmupViewModel<MyBindingContext>())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"{nameof(MyBindingContext)} already warmed up.");

        objectProvider
            .Invoking(sut => sut.WarmupViewModel<EmptyBindingContext>())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"{nameof(EmptyBindingContext)} already warmed up.");
    }

    [Fact]
    public void WarmupAssemblyViewModels_ShouldWarmupAssemblyViewModels()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());

        // Act
        objectProvider.WarmupAssemblyViewModels(Assembly.GetExecutingAssembly());

        // Assert
        objectProvider
            .Invoking(sut => sut.WarmupViewModel<MyBindingContext>())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"{nameof(MyBindingContext)} already warmed up.");

        objectProvider
            .Invoking(sut => sut.WarmupViewModel<EmptyBindingContext>())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"{nameof(EmptyBindingContext)} already warmed up.");
    }

    [Fact]
    public void WarmupViewModelT_ShouldWarmupBindingContext_WhenBindingContextIsNotWarmup()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());

        // Act
        objectProvider.WarmupViewModel<MyBindingContext>();

        // Assert
        objectProvider
            .Invoking(sut => sut.WarmupViewModel<MyBindingContext>())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"{nameof(MyBindingContext)} already warmed up.");
    }

    [Fact]
    public void WarmupViewModel_ShouldWarmupBindingContext_WhenBindingContextIsNotWarmup()
    {
        // Arrange
        var bindingContextType = typeof(MyBindingContext);
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());

        // Act
        objectProvider.WarmupViewModel(bindingContextType);

        // Assert
        objectProvider
            .Invoking(sut => sut.WarmupViewModel(bindingContextType))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"{bindingContextType.Name} already warmed up.");
    }

    [Theory]
    [InlineData(typeof(NoBindingContext))]
    [InlineData(typeof(AbstractBindingContext))]
    [InlineData(typeof(IInterfaceBindingContext))]
    public void WarmupViewModel_ShouldThrow_WhenBindingContextIsNotSupported(Type bindingContextType)
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());

        // Assert
        objectProvider
            .Invoking(sut => sut.WarmupViewModel(bindingContextType))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"Can not warmup {bindingContextType.Name}.");
    }

    [Fact]
    public void WarmupValueConverter_ShouldNotThrow_WhenValueConverterWasNotWarmup()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new IntToStrConverter()
        });

        // Assert
        objectProvider
            .Invoking(sut => sut.WarmupValueConverter<IntToStrConverter>(1))
            .Should()
            .NotThrow();
    }

    [Fact]
    public void WarmupValueConverter_ShouldThrow_WhenValueConverterWasWarmup()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new IntToStrConverter()
        });

        // Act
        objectProvider.WarmupValueConverter<IntToStrConverter>(1);

        // Assert
        objectProvider
            .Invoking(sut => sut.WarmupValueConverter<IntToStrConverter>(1))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Warm up only during the initialization phase.");
    }

    [Fact]
    public void RentProperty_ShouldReturnProperty_WhenDataIsValid()
    {
        // Arrange
        const int countValue = 69;

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext
        {
            Count = countValue,
        };

        IProperty<int> countProperty;
        var countPropertyBindingData = nameof(MyBindingContext.Count).ToPropertyBindingData();

        // Act
        countProperty = objectProvider.RentProperty<int>(bindingContext, countPropertyBindingData);

        // Assert
        countProperty
            .Should()
            .NotBeNull()
            .And
            .BeAssignableTo<IProperty<int>>()
            .And
            .BeAssignableTo<IReadOnlyProperty<int>>();

        countProperty.Value.Should().Be(countValue);
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
    public void RentProperty_ShouldThrow_WhenPropertyIsNotFound()
    {
        // Arrange
        const string notPresentedPropertyName = "NotPresentedProperty";

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        var propertyBindingData = notPresentedPropertyName.ToPropertyBindingData();

        // Assert
        objectProvider
            .Invoking(sut => sut.RentProperty<string>(bindingContext, propertyBindingData))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"Property '{notPresentedPropertyName}' not found.");
    }

    [Fact]
    public void RentProperty_ShouldThrow_WhenPropertyIsReadOnly()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        var titlePropertyBindingData = nameof(MyBindingContext.Title).ToPropertyBindingData();

        // Assert
        objectProvider
            .Invoking(sut => sut.RentProperty<string>(bindingContext, titlePropertyBindingData))
            .Should()
            .Throw<InvalidCastException>();
    }

    [Fact]
    public void RentProperty_ShouldThrow_WhenBindingDataIsNotValid()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());

        var countPropertyBindingData = string.Empty.ToPropertyBindingData();

        // Assert
        objectProvider
            .Invoking(sut => sut.RentProperty<int>(default, default))
            .Should()
            .Throw<NullReferenceException>();

        objectProvider
            .Invoking(sut => sut.RentProperty<int>(default, countPropertyBindingData))
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage(nameof(countPropertyBindingData.PropertyName));
    }

    [Fact]
    public void RentPropertyWithConverter_ShouldThrow_WhenPropertyIsReadOnly()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new IntToStrConverter()
        });

        var bindingContext = new MyBindingContext();

        var intValueBindingData = nameof(MyBindingContext.IntValue).ToPropertyBindingData();

        // Assert
        objectProvider
            .Invoking(sut => sut.RentReadOnlyProperty<string>(bindingContext, intValueBindingData))
            .Should()
            .Throw<InvalidCastException>();
    }

    [Fact]
    public void RentPropertyWithConverter_ShouldThrow_WhenConverterIsNotSet()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        var countPropertyBindingData = nameof(MyBindingContext.Count).ToPropertyBindingData();

        // Assert
        objectProvider
            .Invoking(sut => sut.RentProperty<string>(bindingContext, countPropertyBindingData))
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage($"Property value converter from '{typeof(int)}' to '{typeof(string)}' not found.");
    }

    [Fact]
    public void RentReadOnlyProperty_ShouldReturnProperty_WhenDataIsValid()
    {
        // Arrange
        const string titleValue = "TestTitle";

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext(titleValue);

        IReadOnlyProperty<string> titleProperty;
        var titlePropertyBindingData = nameof(MyBindingContext.Title).ToPropertyBindingData();

        // Act
        titleProperty = objectProvider.RentReadOnlyProperty<string>(bindingContext, titlePropertyBindingData);

        // Assert
        titleProperty
            .Should()
            .NotBeNull()
            .And
            .BeAssignableTo<IReadOnlyProperty<string>>()
            .And
            .NotBeAssignableTo<IProperty<string>>();

        titleProperty.Value.Should().Be(titleValue);
    }

    [Fact]
    public void RentReadOnlyProperty_ShouldThrow_WhenBindingDataIsNotValid()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());

        var countPropertyBindingData = string.Empty.ToPropertyBindingData();

        // Assert
        objectProvider
            .Invoking(sut => sut.RentReadOnlyProperty<int>(default, default))
            .Should()
            .Throw<NullReferenceException>();

        objectProvider
            .Invoking(sut => sut.RentReadOnlyProperty<int>(default, countPropertyBindingData))
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage(nameof(countPropertyBindingData.PropertyName));
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
    public void GetCommand_ShouldThrow_WhenCommandNameIsNull()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        // Assert
        objectProvider
            .Invoking(sut => sut.GetCommand<ICommand>(bindingContext, default))
            .Should()
            .Throw<NullReferenceException>();
    }

    [Fact]
    public void GetCommand_ShouldThrow_WhenCommandIsNotFound()
    {
        // Arrange
        const string notPresentedCommand = "NotPresentedCommand";

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        // Assert
        objectProvider
            .Invoking(sut => sut.GetCommand<ICommand>(bindingContext, notPresentedCommand))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"Command '{notPresentedCommand}' not found.");
    }

    [Fact]
    public void GetCommand_ShouldThrow_WhenMemberTypeIsNotProperty()
    {
        // Arrange
        const string commandName = nameof(MyBindingContext.FieldCommand);

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        // Assert
        objectProvider
            .Invoking(sut => sut.GetCommand<ICommand>(bindingContext, commandName))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"Command '{commandName}' not found.");
    }

    [Fact]
    public void GetCommand_ShouldThrow_WhenCommandTypeIsNotAssignableFromPropertyType()
    {
        // Arrange
        const string commandName = nameof(MyBindingContext.Title);

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        // Assert
        objectProvider
            .Invoking(sut => sut.GetCommand<ICommand>(bindingContext, commandName))
            .Should()
            .Throw<InvalidCastException>()
            .WithMessage(
                $"Can not cast the {typeof(IReadOnlyProperty<string>)} command to the {typeof(ICommand)} command.");
    }

    [Fact]
    public void RentCommandWrapper_ShouldReturnCommand_WhenDataIsValid()
    {
        // Arrange
        const string commandName = nameof(MyBindingContext.SetValueCommand);

        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new ParameterToIntConverter()
        });

        var bindingContext = new MyBindingContext();

        IBaseCommand setValueCommand;
        var setValueCommandBindingData = $"{commandName}, 5".ToCommandBindingData(0);

        // Act
        setValueCommand = objectProvider.RentCommandWrapper(bindingContext, setValueCommandBindingData);

        // Assert
        setValueCommand.Should().NotBeNull();
    }

    [Fact]
    public void RentCommandWrapper_ShouldThrow_WhenParameterIsNull()
    {
        // Arrange
        const string commandName = nameof(MyBindingContext.SetValueCommand);

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        var commandBindingData = $"{commandName}".ToCommandBindingData(0);

        // Assert
        objectProvider
            .Invoking(sut => sut.RentCommandWrapper(bindingContext, commandBindingData))
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage(
                $"Command '{commandName}' has no parameter. Use {nameof(BindingContextObjectProvider.GetCommand)} instead.");
    }

    [Fact]
    public void RentCommandWrapper_ShouldThrow_WhenConverterIsNotSet()
    {
        // Arrange
        const string commandName = nameof(MyBindingContext.SetValueCommand);

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        var commandBindingData = $"{commandName}, 5".ToCommandBindingData(0);

        // Assert
        objectProvider
            .Invoking(sut => sut.RentCommandWrapper(bindingContext, commandBindingData))
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage($"Parameter value converter to '{typeof(int)}' not found.");
    }

    [Fact]
    public void RentCommandWrapper_ShouldThrow_WhenCommandIsNotFound()
    {
        // Arrange
        const string notPresentedCommand = "NotPresentedCommand";

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        var commandBindingData = $"{notPresentedCommand}, 5".ToCommandBindingData(0);

        // Assert
        objectProvider
            .Invoking(sut => sut.RentCommandWrapper(bindingContext, commandBindingData))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"Command '{notPresentedCommand}' not found.");
    }

    [Fact]
    public void RentCommandWrapper_ShouldThrow_WhenMemberTypeIsNotProperty()
    {
        // Arrange
        const string commandName = nameof(MyBindingContext.FieldCommand);

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        var commandBindingData = $"{commandName}, 5".ToCommandBindingData(0);

        // Assert
        objectProvider
            .Invoking(sut => sut.RentCommandWrapper(bindingContext, commandBindingData))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"Command '{commandName}' not found.");
    }

    [Fact]
    public void GetCollectionItemTemplate_ShouldReturnObject_WhenCollectionIsNotEmpty()
    {
        // Arrange
        const int value = 55;

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>(),
            new Dictionary<Type, object>
            {
                { typeof(int), value }
            });

        // Assert
        objectProvider.GetCollectionItemTemplate<int, int>().Should().Be(value);
    }

    [Fact]
    public void GetCollectionItemTemplate_ShouldThrow_WhenCollectionIsEmpty()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());

        // Assert
        objectProvider
            .Invoking(sut => sut.GetCollectionItemTemplate<int, int>())
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage($"Item template for '{typeof(int)}' not found.");
    }

    [Fact]
    public void Dispose_ShouldNotThrow_WhenWarmupViewModelAgain()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());

        // Act
        objectProvider.WarmupViewModel<MyBindingContext>();
        objectProvider.Dispose();

        // Assert
        objectProvider
            .Invoking(sut => sut.WarmupViewModel<MyBindingContext>())
            .Should()
            .NotThrow();
    }

    [Fact]
    public void Dispose_ShouldThrow_WhenWarmupValueConverter()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new IntToStrConverter()
        });

        // Act
        objectProvider.WarmupValueConverter<IntToStrConverter>(1);
        objectProvider.Dispose();

        // Assert
        objectProvider
            .Invoking(sut => sut.WarmupValueConverter<IntToStrConverter>(1))
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage($"Converter '{typeof(IntToStrConverter)}' not found");
    }

    [Fact]
    public void Dispose_ShouldThrow_WhenReturnProperty()
    {
        // Arrange
        var property = new PropertyWrapper<int, string>(default);
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());

        // Act
        objectProvider.Dispose();

        // Assert
        objectProvider
            .Invoking(sut => sut.ReturnProperty(property))
            .Should()
            .Throw<ObjectDisposedException>()
            .WithMessage($"Cannot access a disposed object.\nObject name: '{nameof(ObjectWrapperHandler)}'.");
    }
}