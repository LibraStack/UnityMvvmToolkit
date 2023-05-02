using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Test.Unit.TestCommands;

// ReSharper disable InconsistentNaming

namespace UnityMvvmToolkit.Test.Integration.TestBindingContext;

public class MyBindingContext : IBindingContext
{
    private readonly IProperty<int> _count = new Property<int>();
    
    [Observable(nameof(IntValue))]
    private readonly IReadOnlyProperty<int> m_intValue = new ReadOnlyProperty<int>(69);

    public MyBindingContext(string title = "Title")
    {
        Title = new ReadOnlyProperty<string>(title);

        FieldCommand = new Command(default);

        IncrementCommand = new Command(() => Count++);
        DecrementCommand = new MyCommand(() => Count--);

        SetValueCommand = new Command<int>(value => Count = value);
    }

    public IReadOnlyProperty<string> Title { get; }

    public int Count
    {
        get => _count.Value;
        set => _count.Value = value;
    }

    public int IntValue => m_intValue.Value;

    public ICommand FieldCommand;
    public ICommand IncrementCommand { get; }
    public IMyCommand DecrementCommand { get; }

    public ICommand<int> SetValueCommand { get; }
}