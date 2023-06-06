using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Test.Unit.TestCommands;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace UnityMvvmToolkit.Test.Integration.TestBindingContext;

public class MyBindingContext : IBindingContext
{
    [Observable]
    private readonly IProperty<int> _count = new Property<int>();
    
    [Observable(nameof(IntReadOnlyValue))]
    private readonly IReadOnlyProperty<int> m_intReadOnlyValue = new ReadOnlyProperty<int>(69);

    public MyBindingContext(string title = "Title", int intValue = default)
    {
        Title = new ReadOnlyProperty<string>(title);
        IntReadOnlyProperty = new Property<int>(intValue);

        FieldCommand = new Command(default);

        IncrementCommand = new Command(() => Count++);
        DecrementCommand = new MyCommand(() => Count--);

        SetValueCommand = new Command<int>(value => Count = value);
        SetValueFieldCommand = new Command<int>(value => Count = value);
    }

    public int Count
    {
        get => _count.Value;
        set => _count.Value = value;
    }

    public int IntReadOnlyValue => m_intReadOnlyValue.Value;

    public IReadOnlyProperty<string> Title { get; }

    public IReadOnlyProperty<int> IntReadOnlyProperty { get; }

    public ICommand FieldCommand;
    public ICommand IncrementCommand { get; }
    public IMyCommand DecrementCommand { get; }

    public ICommand<int> SetValueCommand { get; }
    public ICommand<int> SetValueFieldCommand;
}