using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable NotAccessedField.Local

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class PrivateFieldBindingContext : IBindingContext
{
    private IProperty<bool> _bool;
    private IReadOnlyProperty<int> _int;

    private Property<float> _float;
    private ReadOnlyProperty<string> _str;

    public PrivateFieldBindingContext()
    {
        _bool = new Property<bool>();
        _int = new ReadOnlyProperty<int>(0);

        _float = new Property<float>();
        _str = new ReadOnlyProperty<string>(nameof(PrivateFieldBindingContext));
    }
}