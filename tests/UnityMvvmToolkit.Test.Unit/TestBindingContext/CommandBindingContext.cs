using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Test.Unit.TestCommands;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class CommandBindingContext : IBindingContext
{
    public CommandBindingContext()
    {
        incrementCommand = new Command(default);
        _decrementCommand = new MyCommand(default);
        m_multiplyCommand = new Command<int>(default);
        DivideCommand = new MyCommand<int>(default);
    }

    public ICommand incrementCommand { get; }
    public IMyCommand _decrementCommand { get; }

    public ICommand<int> m_multiplyCommand { get; }
    public IMyCommand<int> DivideCommand { get; }
}