using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Test.Unit.TestCommands;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class CommandBindingContext : IBindingContext
{
    public CommandBindingContext()
    {
        IncrementCommand = new Command(default);
        DecrementCommand = new MyCommand(default);
        SetValueCommand = new Command<int>(default);
    }

    public ICommand IncrementCommand { get; }
    public IMyCommand DecrementCommand { get; }

    public ICommand<int> SetValueCommand { get; }
}