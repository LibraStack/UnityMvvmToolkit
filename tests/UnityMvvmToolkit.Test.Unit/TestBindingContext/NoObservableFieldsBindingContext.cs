using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoProperty

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class NoObservableFieldsBindingContext : IBindingContext
{
    private int _count;

    public int Count
    {
        get => _count;
        set => _count = value;
    }
}