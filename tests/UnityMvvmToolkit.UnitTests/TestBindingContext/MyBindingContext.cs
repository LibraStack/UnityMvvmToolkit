using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UnitTests.TestBindingContext;

public class MyBindingContext : IBindingContext
{
    private readonly IProperty<int> _count = new ObservableProperty<int>();
    private readonly IProperty<string> m_description = new ObservableProperty<string>();

    public MyBindingContext()
    {
        Title = new ObservableProperty<string>("Title");

        IncrementCommand = new Command(() => Count++);
        DecrementCommand = new Command(() => Count--);
    }

    public IReadOnlyProperty<string> Title { get; }

    public int Count
    {
        get => _count.Value;
        set => _count.Value = value;
    }

    public string Description
    {
        get => m_description.Value;
        set => m_description.Value = value;
    }

    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }
}