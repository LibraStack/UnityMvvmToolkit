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
using UnityMvvmToolkit.Test.Integration.TestValueConverters;
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
    [InlineData(typeof(NotBindingContext))]
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
    public void TryRentProperty_ShouldReturnProperty_WhenDataIsValid()
    {
        // Arrange
        const int countValue = 69;

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext
        {
            Count = countValue,
        };

        var countPropertyBindingData = nameof(MyBindingContext.Count).ToPropertyBindingData();

        // Act
        var result =
            objectProvider.TryRentProperty<int>(bindingContext, countPropertyBindingData, out var countProperty);

        // Assert
        countProperty
            .Should()
            .NotBeNull()
            .And
            .BeAssignableTo<IProperty<int>>()
            .And
            .BeAssignableTo<IReadOnlyProperty<int>>();

        result.Should().BeTrue();
        countProperty.Value.Should().Be(countValue);
    }

    [Fact]
    public void TryRentProperty_ShouldNotReturnProperty_WhenPropertyIsReadOnly()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        var readOnlyPropertyBindingData = nameof(MyBindingContext.IntReadOnlyValue).ToPropertyBindingData();

        // Act
        var result =
            objectProvider.TryRentProperty<int>(bindingContext, readOnlyPropertyBindingData, out var countProperty);

        // Assert
        result.Should().BeFalse();
        countProperty.Should().BeNull();
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

        var countPropertyBindingData = nameof(MyBindingContext.Count).ToPropertyBindingData();

        // Act
        var countProperty = objectProvider.RentProperty<int>(bindingContext, countPropertyBindingData);

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
    public void RentProperty_ShouldReturnProperty_WhenConverterIsSet()
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

        var countPropertyBindingData = nameof(MyBindingContext.Count).ToPropertyBindingData();

        // Act
        var countProperty = objectProvider.RentProperty<string>(bindingContext, countPropertyBindingData);

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
    public void RentProperty_ShouldReturnValidPropertyWrapper()
    {
        // Arrange
        const int countValue = 69;

        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new IntToStrConverter()
        });

        var bindingContext = new MyBindingContext(intValue: countValue)
        {
            Count = countValue
        };

        var countPropertyBindingData = nameof(MyBindingContext.Count).ToPropertyBindingData();
        var countReadOnlyPropertyBindingData = nameof(MyBindingContext.IntReadOnlyProperty).ToPropertyBindingData();

        // Act
        var countProperty = objectProvider.RentProperty<string>(bindingContext, countPropertyBindingData);
        var countReadOnlyProperty = objectProvider.RentReadOnlyProperty<string>(bindingContext, countReadOnlyPropertyBindingData);

        objectProvider.ReturnReadOnlyProperty(countReadOnlyProperty);
        objectProvider.ReturnProperty(countProperty);

        countProperty = objectProvider.RentProperty<string>(bindingContext, countPropertyBindingData);
        countReadOnlyProperty = objectProvider.RentReadOnlyProperty<string>(bindingContext, countReadOnlyPropertyBindingData);

        // Assert

        countProperty.Value.Should().Be(countValue.ToString());
        countReadOnlyProperty.Value.Should().Be(countValue.ToString());
    }

    [Fact]
    public void RentProperty_ShouldConvertPropertyValue_WhenSourceAndTargetTypesMatch()
    {
        // Arrange
        const bool resultBoolValue = true;

        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new InvertBoolPropConverter()
        });

        var bindingContext = new MyBindingContext();

        var boolPropertyBindingData = $"BoolProperty, {nameof(InvertBoolPropConverter)}".ToPropertyBindingData();

        // Act
        var boolProperty = objectProvider.RentProperty<bool>(bindingContext, boolPropertyBindingData);

        // Assert
        boolProperty
            .Should()
            .NotBeNull()
            .And
            .BeAssignableTo<IProperty<bool>>()
            .And
            .BeAssignableTo<IReadOnlyProperty<bool>>();

        boolProperty.Value.Should().Be(resultBoolValue);
    }

    [Fact]
    public void RentProperty_ShouldReturnReadOnlyProperty_WhenPropertyInstanceIsNotReadOnly()
    {
        // Arrange
        const int intValue = 25;

        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new IntToStrConverter()
        });

        var bindingContext = new MyBindingContext(intValue: intValue);

        var intPropertyBindingData = nameof(MyBindingContext.IntFakeReadOnlyProperty).ToPropertyBindingData();

        // Act
        IReadOnlyProperty<int> intProperty = objectProvider.RentProperty<int>(bindingContext, intPropertyBindingData);

        // Assert
        intProperty
            .Should()
            .NotBeNull()
            .And
            .BeAssignableTo<IProperty<int>>()
            .And
            .BeAssignableTo<IReadOnlyProperty<int>>();

        intProperty.Value.Should().Be(intValue);
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
    public void RentProperty_ShouldThrow_WhenPropertyTypeIsWrong()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        var propertyBindingData = nameof(MyBindingContext.IncrementCommand).ToPropertyBindingData();

        // Assert
        objectProvider
            .Invoking(sut => sut.RentProperty<string>(bindingContext, propertyBindingData))
            .Should()
            .Throw<InvalidCastException>()
            .WithMessage($"Unable to cast object of type '{typeof(Command)}' to type '{typeof(IBaseProperty)}'.");
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
    public void RentProperty_ShouldThrow_WhenConverterIsNotSet()
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
    public void RentPropertyAs_ShouldReturnProperty_WhenDataIsValid()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new ParentBindingContext();

        var nestedPropertyBindingData =
            nameof(ParentBindingContext.NestedPropertyBindingContext).ToPropertyBindingData();

        // Act
        var nestedProperty =
            objectProvider.RentPropertyAs<IBindingContext>(bindingContext, nestedPropertyBindingData);

        // Assert
        nestedProperty
            .Should()
            .NotBeNull()
            .And
            .BeAssignableTo<IProperty<IBindingContext>>()
            .And
            .BeAssignableTo<IReadOnlyProperty<IBindingContext>>();
    }

    [Fact]
    public void RentPropertyAs_ShouldReturnValueTypeProperty_WhenDataIsValid()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new ParentBindingContext();

        var floatPropertyBindingData =
            nameof(ParentBindingContext.FloatPropertyBindingContext).ToPropertyBindingData();

        // Act
        var floatProperty = objectProvider.RentPropertyAs<float>(bindingContext, floatPropertyBindingData);

        // Assert
        floatProperty
            .Should()
            .NotBeNull()
            .And
            .BeAssignableTo<IProperty<float>>()
            .And
            .BeAssignableTo<IReadOnlyProperty<float>>();
    }

    [Fact]
    public void RentPropertyAs_ShouldThrow_WhenTargetTypeIsNotAssignableFromSourceType()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new ParentBindingContext();

        var nestedPropertyBindingData = nameof(ParentBindingContext.NestedProperty).ToPropertyBindingData();

        // Assert
        objectProvider
            .Invoking(sut => sut.RentPropertyAs<IBindingContext>(bindingContext, nestedPropertyBindingData))
            .Should()
            .Throw<InvalidCastException>()
            .WithMessage($"Can not cast the '{typeof(NestedClass)}' to the '{typeof(IBindingContext)}'.");
    }

    [Fact]
    public void RentPropertyAs_ShouldThrow_WheTryingToCastValueTypes()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new ParentBindingContext();

        var floatPropertyBindingData = nameof(ParentBindingContext.FloatPropertyBindingContext).ToPropertyBindingData();

        // Assert
        objectProvider
            .Invoking(sut => sut.RentPropertyAs<int>(bindingContext, floatPropertyBindingData))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage(
                $"{nameof(ObjectWrapperHandler.GetPropertyAs)} is not supported for value types. Use {typeof(PropertyValueConverter<,>).Name} instead.");
    }

    [Fact]
    public void RentReadOnlyProperty_ShouldReturnProperty_WhenDataIsValid()
    {
        // Arrange
        const string titleValue = "TestTitle";

        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext(titleValue);

        var titlePropertyBindingData = nameof(MyBindingContext.Title).ToPropertyBindingData();

        // Act
        var titleProperty = objectProvider.RentReadOnlyProperty<string>(bindingContext, titlePropertyBindingData);

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
    public void RentReadOnlyProperty_ShouldConvertPropertyValue_WhenSourceAndTargetTypesMatch()
    {
        // Arrange
        const bool resultBoolValue = true;

        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new InvertBoolPropConverter()
        });

        var bindingContext = new MyBindingContext();

        var boolPropertyBindingData =
            $"BoolReadOnlyProperty, {nameof(InvertBoolPropConverter)}".ToPropertyBindingData();

        // Act
        var boolProperty = objectProvider.RentReadOnlyProperty<bool>(bindingContext, boolPropertyBindingData);

        // Assert
        boolProperty
            .Should()
            .NotBeNull()
            .And
            .NotBeAssignableTo<IProperty<bool>>()
            .And
            .BeAssignableTo<IReadOnlyProperty<bool>>();

        boolProperty.Value.Should().Be(resultBoolValue);
    }

    [Fact]
    public void RentReadOnlyPropertyAs_ShouldReturnProperty_WhenDataIsValid()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new ParentBindingContext();

        var nestedReadOnlyPropertyBindingData =
            nameof(ParentBindingContext.NestedReadOnlyPropertyBindingContext).ToPropertyBindingData();

        // Act
        var nestedReadOnlyProperty =
            objectProvider.RentReadOnlyPropertyAs<IBindingContext>(bindingContext, nestedReadOnlyPropertyBindingData);

        // Assert
        nestedReadOnlyProperty
            .Should()
            .NotBeNull()
            .And
            .BeAssignableTo<IReadOnlyProperty<IBindingContext>>()
            .And
            .NotBeAssignableTo<IProperty<IBindingContext>>();
    }

    [Fact]
    public void RentReadOnlyProperty_ShouldThrow_WhenPropertyTypeIsWrong()
    {
        // Arrange
        var objectProvider = new BindingContextObjectProvider(Array.Empty<IValueConverter>());
        var bindingContext = new MyBindingContext();

        var propertyBindingData = nameof(MyBindingContext.FieldCommand).ToPropertyBindingData();

        // Assert
        objectProvider
            .Invoking(sut => sut.RentReadOnlyProperty<string>(bindingContext, propertyBindingData))
            .Should()
            .Throw<InvalidCastException>()
            .WithMessage($"Unable to cast object of type '{typeof(Command)}' to type '{typeof(IBaseProperty)}'.");
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

        // Act
        var fieldCommand = objectProvider
            .GetCommand<ICommand>(bindingContext, nameof(MyBindingContext.FieldCommand));

        var incrementCommand = objectProvider
            .GetCommand<ICommand>(bindingContext, nameof(MyBindingContext.IncrementCommand));

        var decrementCommand = objectProvider
            .GetCommand<ICommand>(bindingContext, nameof(MyBindingContext.DecrementCommand));

        // Assert
        fieldCommand.Should().NotBeNull().And.BeAssignableTo<IBaseCommand>();
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
                $"Unable to cast object of type '{typeof(ReadOnlyProperty<string>)}' to type '{typeof(ICommand)}'.");
    }

    [Fact]
    public void RentCommandWrapper_ShouldReturnCommand_WhenDataIsValid()
    {
        // Arrange
        const string commandName = nameof(MyBindingContext.SetValueCommand);
        const string fieldCommandName = nameof(MyBindingContext.SetValueFieldCommand);

        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new ParameterToIntConverter()
        });

        var bindingContext = new MyBindingContext();

        var setValueCommandBindingData = $"{commandName}, 5".ToCommandBindingData(0);
        var setValueFieldCommandBindingData = $"{fieldCommandName}, 5".ToCommandBindingData(0);

        // Act
        var setValueCommand = objectProvider.RentCommandWrapper(bindingContext, setValueCommandBindingData);
        var setValueFieldCommand = objectProvider.RentCommandWrapper(bindingContext, setValueFieldCommandBindingData);

        // Assert
        setValueCommand.Should().NotBeNull();
        setValueFieldCommand.Should().NotBeNull();
    }

    [Fact]
    public void RentCommandWrapper_ShouldReturnCommand_WithInvertedParameterValue()
    {
        // Arrange
        const bool resultBoolValue = true;

        var objectProvider = new BindingContextObjectProvider(new IValueConverter[]
        {
            new InvertBoolParamConverter()
        });

        var bindingContext = new MyBindingContext();

        var boolCommandBindingData =
            $"BoolCommand, {bool.FalseString}, {nameof(InvertBoolParamConverter)}".ToCommandBindingData(0);

        // Act
        var boolCommand = objectProvider.RentCommandWrapper(bindingContext, boolCommandBindingData);
        boolCommand.Execute(0);

        // Assert
        bindingContext.BoolValue.Should().Be(resultBoolValue);
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
        var property = new PropertyConvertWrapper<int, string>(default);
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