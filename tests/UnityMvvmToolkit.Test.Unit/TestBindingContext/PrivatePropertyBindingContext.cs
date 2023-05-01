using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable InconsistentNaming

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class PrivatePropertyBindingContext : IBindingContext
{
    public PrivatePropertyBindingContext()
    {
        boolProperty = new Property<bool>();
        _intProperty = new ReadOnlyProperty<int>(0);

        m_floatProperty = new Property<float>();
        StrProperty = new ReadOnlyProperty<string>(nameof(PrivatePropertyBindingContext));
    }

    private IProperty<bool> boolProperty { get; }
    private IReadOnlyProperty<int> _intProperty { get; }
    private Property<float> m_floatProperty { get; }
    private ReadOnlyProperty<string> StrProperty { get; }
}