using FluentAssertions;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Converters.ParameterValueConverters;
using UnityMvvmToolkit.Core.Internal.ObjectWrappers;

namespace UnityMvvmToolkit.Test.Unit;

public class CommandWrapperTests
{
    private readonly ParameterToIntConverter _parameterToIntConverter;

    public CommandWrapperTests()
    {
        _parameterToIntConverter = new ParameterToIntConverter();
    }

    [Fact]
    public void CommandWrapper_ShouldCreateNewCommandWrapperInstance()
    {
        // Act
        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        // Assert
        commandWrapper.CommandId.Should().Be(-1);
        commandWrapper.ConverterId.Should().Be(-1);
    }

    [Fact]
    public void Execute_ShouldInvokeAction_WhenActionIsNotNull()
    {
        // Arrange
        const int commandId = 0;
        const int elementId = 0;
        const string commandParameter = "69";

        var result = 0;

        var command = new Command<int>(parameter => result = parameter);
        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        // Act
        commandWrapper
            .SetCommand(commandId, command)
            .RegisterParameter(elementId, commandParameter)
            .Execute(elementId);

        // Assert
        result.Should().Be(_parameterToIntConverter.Convert(commandParameter));
    }

    [Fact]
    public void SetConverterId_ShouldSetButNotResetConverterId_WhenConverterIdIsNotSet()
    {
        // Arrange
        const int commandId = 0;
        const int converterId = 5;

        var command = new Command<int>(default);
        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        // Act
        commandWrapper
            .SetConverterId(converterId)
            .SetCommand(commandId, command);

        commandWrapper.Reset();

        // Assert
        commandWrapper.ConverterId.Should().Be(converterId);
    }

    [Fact]
    public void SetConverterId_ShouldThrow_WhenConverterIdAlreadySet()
    {
        // Arrange
        const int converterId = 5;

        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        // Act
        commandWrapper.SetConverterId(converterId);

        // Assert
        commandWrapper
            .Invoking(sut => sut.SetConverterId(converterId))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Can not change converter ID.");
    }

    [Fact]
    public void SetCommand_ShouldSetCommand_WhenCommandIsNotSet()
    {
        // Arrange
        const int commandId = 0;

        var command = new Command<int>(default);
        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        // Act
        commandWrapper.SetCommand(commandId, command);

        // Assert
        commandWrapper.CommandId.Should().Be(commandId);
        commandWrapper
            .Invoking(sut => sut.Reset())
            .Should()
            .NotThrow();
    }

    [Fact]
    public void SetCommand_ShouldThrow_WhenCommandIsNotValid()
    {
        // Arrange
        const int commandId = 0;

        var command = new Command(default);
        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        // Assert
        commandWrapper
            .Invoking(sut => sut.SetCommand(commandId, command))
            .Should()
            .Throw<InvalidCastException>();
    }

    [Fact]
    public void SetCommand_ShouldThrow_WhenCommandAlreadySet()
    {
        // Arrange
        const int commandId = 0;

        var command = new Command<int>(default);
        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        // Act
        commandWrapper.SetCommand(commandId, command);

        // Assert
        commandWrapper
            .Invoking(sut => sut.SetCommand(commandId, command))
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage($"{nameof(commandWrapper)} was not reset.");
    }

    [Fact]
    public void RegisterParameter_ShouldRegisterParameter_WhenParameterIsNotRegistered()
    {
        // Arrange
        const int elementId = 0;
        const string commandParameter = "69";

        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        // Act
        commandWrapper.RegisterParameter(elementId, commandParameter);

        // Assert
        commandWrapper.UnregisterParameter(elementId).Should().Be(0);
    }

    [Fact]
    public void RegisterParameter_ShouldThrow_WhenParameterAlreadyRegistered()
    {
        // Arrange
        const int elementId = 0;
        const string commandParameter = "69";

        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        // Act
        commandWrapper.RegisterParameter(elementId, commandParameter);

        // Assert
        commandWrapper
            .Invoking(sut => sut.RegisterParameter(elementId, commandParameter))
            .Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void UnregisterParameter_ShouldNotThrow_WhenParameterIsNotRegistered()
    {
        // Arrange
        const int elementId = 0;

        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        // Assert
        commandWrapper.UnregisterParameter(elementId).Should().Be(0);
    }

    [Fact]
    public void CanExecute_ShouldReturnCorrectValue_WhenCanExecuteActionIsNotNull()
    {
        // Arrange
        const int commandId = 0;
        const bool canExecute = true;

        var command = new Command<int>(default, () => canExecute);
        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        // Act
        commandWrapper.SetCommand(commandId, command);

        // Assert
        commandWrapper.CanExecute().Should().Be(true);
    }

    [Fact]
    public void RaiseCanExecuteChanged_ShouldRaiseCommandCanExecuteChanged_WhenCommandWrapperCanExecuteChanged()
    {
        // Arrange
        const int commandId = 0;

        var raisedCount = 0;

        bool CanExecuteFunc() => true;

        var command = new Command<int>(default, CanExecuteFunc);
        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        command.CanExecuteChanged += (_, _) => { raisedCount++; };

        // Act
        commandWrapper.SetCommand(commandId, command);
        commandWrapper.RaiseCanExecuteChanged();

        // Assert
        raisedCount.Should().Be(1);
    }

    [Fact]
    public void RaiseCanExecuteChanged_ShouldRaiseCommandWrapperCanExecuteChanged_WhenCommandCanExecuteChanged()
    {
        // Arrange
        const int commandId = 0;

        var raisedCount = 0;

        bool CanExecuteFunc() => true;

        var command = new Command<int>(default, CanExecuteFunc);
        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        commandWrapper.CanExecuteChanged += (_, _) => { raisedCount++; };

        // Act
        commandWrapper.SetCommand(commandId, command);
        command.RaiseCanExecuteChanged();

        // Assert
        raisedCount.Should().Be(1);
    }

    [Fact]
    public void Reset_ShouldResetCommandAndCommandId_WhenCommandIsNotNull()
    {
        // Arrange
        const int commandId = 0;

        var raisedCount = 0;

        bool CanExecuteFunc() => true;

        var command = new Command<int>(default, CanExecuteFunc);
        var commandWrapper = new CommandWrapper<int>(_parameterToIntConverter);

        commandWrapper.CanExecuteChanged += (_, _) => { raisedCount++; };

        // Act
        commandWrapper.SetCommand(commandId, command);
        commandWrapper.Reset();

        command.RaiseCanExecuteChanged();

        // Assert
        raisedCount.Should().Be(0);
        commandWrapper.CommandId.Should().Be(-1);
    }
}