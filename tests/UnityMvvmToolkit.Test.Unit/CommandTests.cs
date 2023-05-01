using FluentAssertions;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable RedundantAssignment

namespace UnityMvvmToolkit.Test.Unit;

public class CommandTests
{
    [Fact]
    public void Execute_ShouldInvokeAction_WhenActionIsNotNull()
    {
        // Arrange
        var result = 0;

        var command = new Command(() => result++);

        // Act
        command.Execute();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void Execute_ShouldInvokeActionWithoutParameter_WhenCastToICommand()
    {
        // Arrange
        var result = 0;

        ICommand command = new Command(() => result++);

        // Act
        command.Execute(elementId: 0);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void Execute_ShouldInvokeActionWithParameter_WhenActionIsNotNull()
    {
        // Arrange
        const int parameterValue = 55;

        var result = 0;

        var command = new Command<int>(value => result = value);

        // Act
        command.Execute(parameterValue);

        // Assert
        result.Should().Be(parameterValue);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenActionIsNull()
    {
        // Arrange
        var command = new Command(default);

        // Assert
        command
            .Invoking(sut => sut.Execute())
            .Should()
            .Throw<NullReferenceException>();
    }

    [Fact]
    public void Execute_ShouldThrow_WhenActionWithParameterIsNull()
    {
        // Arrange
        var command = new Command<int>(default);

        // Assert
        command
            .Invoking(sut => sut.Execute(default))
            .Should()
            .Throw<NullReferenceException>();
    }

    [Fact]
    public void Execute_ShouldThrow_WhenNotImplemented()
    {
        // Arrange
        var command = new Command<int>(default);

        // Assert
        command
            .Invoking(sut => ((IBaseCommand) sut).Execute(default))
            .Should()
            .Throw<NotImplementedException>();
    }

    [Fact]
    public void CanExecute_ShouldReturnTrue_WhenCanExecuteActionIsNull()
    {
        // Arrange
        var command = new Command(default);

        // Assert
        command.CanExecute().Should().Be(true);
    }

    [Fact]
    public void CanExecute_ShouldReturnCorrectValue_WhenCanExecuteActionIsNotNull()
    {
        // Arrange
        var command = new Command(default, () => false);

        // Assert
        command.CanExecute().Should().Be(false);
    }

    [Fact]
    public void RaiseCanExecuteChanged_ShouldRaiseCanExecuteChanged_WhenCanExecuteActionChanged()
    {
        // Arrange
        var raisedCount = 0;

        bool CanExecuteFunc() => true;

        var command = new Command(default, CanExecuteFunc);

        command.CanExecuteChanged += (_, _) => { raisedCount++; };

        // Act
        command.RaiseCanExecuteChanged();

        // Assert
        raisedCount.Should().Be(1);
    }

    [Fact]
    public void RaiseCanExecuteChanged_ShouldNotRaiseCanExecuteChanged_WhenCanExecuteActionReturnSameValue()
    {
        // Arrange
        var raisedCount = 0;

        bool CanExecuteFunc() => true;

        var command = new Command(default, CanExecuteFunc);

        command.CanExecuteChanged += (_, _) => { raisedCount++; };

        // Act
        command.RaiseCanExecuteChanged();
        command.RaiseCanExecuteChanged();

        // Assert
        raisedCount.Should().Be(1);
    }

    [Fact]
    public void RaiseCanExecuteChanged_ShouldRaiseCanExecuteChanged_WhenCanExecuteActionReturnNewValue()
    {
        // Arrange
        var raisedCount = 0;
        var canExecute = false;

        bool CanExecuteFunc() => canExecute;

        var command = new Command(default, CanExecuteFunc);

        command.CanExecuteChanged += (_, _) => { raisedCount++; };

        // Act
        command.RaiseCanExecuteChanged();
        canExecute = true;
        command.RaiseCanExecuteChanged();

        // Assert
        raisedCount.Should().Be(2);
    }
}