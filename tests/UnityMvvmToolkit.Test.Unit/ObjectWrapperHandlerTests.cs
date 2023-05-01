using FluentAssertions;
using NSubstitute;
using UnityMvvmToolkit.Core.Converters.ParameterValueConverters;
using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;
using UnityMvvmToolkit.Core.Enums;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Helpers;
using UnityMvvmToolkit.Core.Internal.Interfaces;
using UnityMvvmToolkit.Core.Internal.ObjectHandlers;

namespace UnityMvvmToolkit.Test.Unit;

public class ObjectWrapperHandlerTests
{
    private readonly IntToStrConverter _intToStrConverter;
    private readonly ParameterToIntConverter _parameterToIntConverter;

    private readonly ValueConverterHandler _valueConverterHandler;

    public ObjectWrapperHandlerTests()
    {
        _intToStrConverter = new IntToStrConverter();
        _parameterToIntConverter = new ParameterToIntConverter();

        _valueConverterHandler = new ValueConverterHandler(new IValueConverter[]
        {
            _intToStrConverter,
            _parameterToIntConverter
        });
    }

    [Fact]
    public void CreateValueConverterInstances_ShouldCreatePropertyWrapperWithConverterByType_WhenConverterRegistered()
    {
        // Arrange
        var converterId = HashCodeHelper.GetPropertyWrapperConverterId(_intToStrConverter);

        var propertyWrapper = Substitute.For<IPropertyWrapper>();
        propertyWrapper.ConverterId.Returns(converterId);

        var objectWrapperHandler = new ObjectWrapperHandler(_valueConverterHandler);

        // Act
        objectWrapperHandler.CreateValueConverterInstances<IntToStrConverter>(1, WarmupType.OnlyByType);

        // Assert
        objectWrapperHandler
            .Invoking(sut => sut.ReturnProperty(propertyWrapper))
            .Should()
            .NotThrow();
    }

    [Fact]
    public void CreateValueConverterInstances_ShouldCreatePropertyWrapperWithConverterByName_WhenConverterRegistered()
    {
        // Arrange
        var converterId = HashCodeHelper.GetPropertyWrapperConverterId(_intToStrConverter, _intToStrConverter.Name);

        var propertyWrapper = Substitute.For<IPropertyWrapper>();
        propertyWrapper.ConverterId.Returns(converterId);

        var objectWrapperHandler = new ObjectWrapperHandler(_valueConverterHandler);

        // Act
        objectWrapperHandler.CreateValueConverterInstances<IntToStrConverter>(1, WarmupType.OnlyByName);

        // Assert
        objectWrapperHandler
            .Invoking(sut => sut.ReturnProperty(propertyWrapper))
            .Should()
            .NotThrow();
    }

    [Fact]
    public void
        CreateValueConverterInstances_ShouldCreatePropertyWrapperWithConvertersByTypeAndName_WhenConverterRegistered()
    {
        // Arrange
        var converterIdByType = HashCodeHelper.GetPropertyWrapperConverterId(_intToStrConverter);
        var converterIdByName =
            HashCodeHelper.GetPropertyWrapperConverterId(_intToStrConverter, _intToStrConverter.Name);

        var propertyWrapperByType = Substitute.For<IPropertyWrapper>();
        propertyWrapperByType.ConverterId.Returns(converterIdByType);

        var propertyWrapperByName = Substitute.For<IPropertyWrapper>();
        propertyWrapperByName.ConverterId.Returns(converterIdByName);

        var objectWrapperHandler = new ObjectWrapperHandler(_valueConverterHandler);

        // Act
        objectWrapperHandler.CreateValueConverterInstances<IntToStrConverter>(1, WarmupType.ByTypeAndName);

        // Assert
        objectWrapperHandler
            .Invoking(sut => sut.ReturnProperty(propertyWrapperByType))
            .Should()
            .NotThrow();

        objectWrapperHandler
            .Invoking(sut => sut.ReturnProperty(propertyWrapperByName))
            .Should()
            .NotThrow();
    }

    [Fact]
    public void CreateValueConverterInstances_ShouldCreateCommandWrapperWithConverterByType_WhenConverterRegistered()
    {
        // Arrange
        const int elementId = 55;
        var converterId = HashCodeHelper.GetCommandWrapperConverterId(_parameterToIntConverter);

        var commandWrapper = Substitute.For<ICommandWrapper>();
        commandWrapper.ConverterId.Returns(converterId);

        var objectWrapperHandler = new ObjectWrapperHandler(_valueConverterHandler);

        // Act
        objectWrapperHandler.CreateValueConverterInstances<ParameterToIntConverter>(1, WarmupType.OnlyByType);

        // Assert
        objectWrapperHandler
            .Invoking(sut => sut.ReturnCommandWrapper(commandWrapper, elementId))
            .Should()
            .NotThrow();
    }

    [Fact]
    public void CreateValueConverterInstances_ShouldCreateCommandWrapperWithConverterByName_WhenConverterRegistered()
    {
        // Arrange
        const int elementId = 55;
        var converterId =
            HashCodeHelper.GetCommandWrapperConverterId(_parameterToIntConverter, _parameterToIntConverter.Name);

        var commandWrapper = Substitute.For<ICommandWrapper>();
        commandWrapper.ConverterId.Returns(converterId);

        var objectWrapperHandler = new ObjectWrapperHandler(_valueConverterHandler);

        // Act
        objectWrapperHandler.CreateValueConverterInstances<ParameterToIntConverter>(1, WarmupType.OnlyByName);

        // Assert
        objectWrapperHandler
            .Invoking(sut => sut.ReturnCommandWrapper(commandWrapper, elementId))
            .Should()
            .NotThrow();
    }

    [Fact]
    public void
        CreateValueConverterInstances_ShouldCreateCommandWrapperWithConverterByTypeAndName_WhenConverterRegistered()
    {
        // Arrange
        const int elementId = 55;
        var converterIdByType =
            HashCodeHelper.GetCommandWrapperConverterId(_parameterToIntConverter);
        var converterIdByName =
            HashCodeHelper.GetCommandWrapperConverterId(_parameterToIntConverter, _parameterToIntConverter.Name);

        var commandWrapperByType = Substitute.For<ICommandWrapper>();
        commandWrapperByType.ConverterId.Returns(converterIdByType);

        var commandWrapperByName = Substitute.For<ICommandWrapper>();
        commandWrapperByName.ConverterId.Returns(converterIdByName);

        var objectWrapperHandler = new ObjectWrapperHandler(_valueConverterHandler);

        // Act
        objectWrapperHandler.CreateValueConverterInstances<ParameterToIntConverter>(1, WarmupType.ByTypeAndName);

        // Assert
        objectWrapperHandler
            .Invoking(sut => sut.ReturnCommandWrapper(commandWrapperByType, elementId))
            .Should()
            .NotThrow();

        objectWrapperHandler
            .Invoking(sut => sut.ReturnCommandWrapper(commandWrapperByName, elementId))
            .Should()
            .NotThrow();
    }

    [Fact]
    public void CreateValueConverterInstances_ShouldThrow_WhenConverterIsNotRegistered()
    {
        // Arrange
        var objectWrapperHandler = new ObjectWrapperHandler(_valueConverterHandler);

        // Assert
        objectWrapperHandler
            .Invoking(sut => sut.CreateValueConverterInstances<FloatToStrConverter>(1, WarmupType.OnlyByName))
            .Should()
            .Throw<NullReferenceException>()
            .WithMessage($"Converter '{typeof(FloatToStrConverter)}' not found");
    }

    [Fact]
    public void ReturnProperty_ShouldResetPropertyWrapper_WhenPropertyWrapperWasReturnToPool()
    {
        // Arrange
        var converterId = HashCodeHelper.GetPropertyWrapperConverterId(_intToStrConverter);

        var propertyWrapper = Substitute.For<IPropertyWrapper>();
        propertyWrapper.ConverterId.Returns(converterId);

        var objectWrapperHandler = new ObjectWrapperHandler(_valueConverterHandler);

        // Act
        objectWrapperHandler.CreateValueConverterInstances<IntToStrConverter>(1, WarmupType.OnlyByType);
        objectWrapperHandler.ReturnProperty(propertyWrapper);

        // Assert
        propertyWrapper.Received(1).Reset();
    }

    [Fact]
    public void ReturnProperty_ShouldThrow_WhenHandlerWasDispose()
    {
        // Arrange
        var objectWrapperHandler = new ObjectWrapperHandler(_valueConverterHandler);

        // Act
        objectWrapperHandler.Dispose();

        // Assert
        objectWrapperHandler
            .Invoking(sut => sut.ReturnProperty(default))
            .Should()
            .Throw<ObjectDisposedException>()
            .WithMessage($"Cannot access a disposed object.\nObject name: '{nameof(ObjectWrapperHandler)}'.");
    }

    [Fact]
    public void ReturnCommand_ShouldResetCommandWrapper_WhenCommandWrapperWasReturnToPool()
    {
        // Arrange
        const int elementId = 0;
        var converterId = HashCodeHelper.GetCommandWrapperConverterId(_parameterToIntConverter);

        var commandWrapper = Substitute.For<ICommandWrapper>();
        commandWrapper.ConverterId.Returns(converterId);

        var objectWrapperHandler = new ObjectWrapperHandler(_valueConverterHandler);

        // Act
        objectWrapperHandler.CreateValueConverterInstances<ParameterToIntConverter>(1, WarmupType.OnlyByType);
        objectWrapperHandler.ReturnCommandWrapper(commandWrapper, elementId);

        // Assert
        commandWrapper.Received(1).Reset();
    }

    [Fact]
    public void ReturnCommand_ShouldThrow_WhenHandlerWasDispose()
    {
        // Arrange
        var objectWrapperHandler = new ObjectWrapperHandler(_valueConverterHandler);

        // Act
        objectWrapperHandler.Dispose();

        // Assert
        objectWrapperHandler
            .Invoking(sut => sut.ReturnCommandWrapper(default, default))
            .Should()
            .Throw<ObjectDisposedException>()
            .WithMessage($"Cannot access a disposed object.\nObject name: '{nameof(ObjectWrapperHandler)}'.");
    }
}