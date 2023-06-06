using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Test.Unit.TestCommands;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace UnityMvvmToolkit.Test.Integration.TestBindingContext;

public class MyBindingContext : IBindingContext
{
    [Observable]
    private readonly IProperty<int> _count = new Property<int>();
    
    [Observable(nameof(IntReadOnlyValue))]
    private readonly IReadOnlyProperty<int> m_intReadOnlyValue = new ReadOnlyProperty<int>(69);

    [Observable]
    private readonly IProperty<bool> _boolProperty = new Property<bool>(false);

    [Observable]
    private readonly IReadOnlyProperty<bool> _boolReadOnlyProperty = new ReadOnlyProperty<bool>(false);

    [Observable]
    private readonly ICommand<bool> _boolCommand;

    public MyBindingContext(string title = "Title", int intValue = default)
    {
        _boolCommand = new Command<bool>(value => BoolValue = value);

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

    public bool BoolValue { get; private set; }

    public int IntReadOnlyValue => m_intReadOnlyValue.Value;

    public IReadOnlyProperty<string> Title { get; }

    public IReadOnlyProperty<int> IntReadOnlyProperty { get; }

    public ICommand FieldCommand;
    public ICommand IncrementCommand { get; }
    public IMyCommand DecrementCommand { get; }

    public ICommand<int> SetValueCommand { get; }
    public ICommand<int> SetValueFieldCommand;
}