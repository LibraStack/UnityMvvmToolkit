using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Test.Unit.TestCommands;

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Local
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class CommandBindingContext : IBindingContext
{
    public CommandBindingContext()
    {
        _privateFieldCommand = new Command(default);
        m_protectedFieldCommand = new Command(default);
        _observablePrivateCommand = new Command(default);

        _privatePropertyCommand = new Command(default);
        m_protectedPropertyCommand = new Command(default);

        incrementCommand = new Command(default);
        _decrementCommand = new MyCommand(default);
        m_multiplyCommand = new Command<int>(default);
        DivideCommand = new MyCommand<int>(default);

        ObservablePublicCommand = new Command(default);
    }

    [Observable]
    private ICommand _privateFieldCommand;

    [Observable]
    protected ICommand m_protectedFieldCommand;

    [Observable("PrivateCommand")]
    private ICommand _observablePrivateCommand;

    [Observable]
    private ICommand _privatePropertyCommand { get; }

    [Observable]
    protected ICommand m_protectedPropertyCommand { get; }

    public ICommand incrementCommand { get; }
    public IMyCommand _decrementCommand { get; }

    public ICommand<int> m_multiplyCommand { get; }
    public IMyCommand<int> DivideCommand { get; }

    [Observable("ObservableCommand")]
    public ICommand ObservablePublicCommand { get; }
}