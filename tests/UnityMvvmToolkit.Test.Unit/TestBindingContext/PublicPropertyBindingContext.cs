using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable InconsistentNaming

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class PublicPropertyBindingContext : IBindingContext
{
    public PublicPropertyBindingContext()
    {
        boolProperty = new Property<bool>();
        _intProperty = new ReadOnlyProperty<int>(0);

        m_floatProperty = new Property<float>();
        StrProperty = new ReadOnlyProperty<string>(nameof(PublicPropertyBindingContext));
    }

    public IProperty<bool> boolProperty { get; }
    public IReadOnlyProperty<int> _intProperty { get; }
    public Property<float> m_floatProperty { get; }
    public ReadOnlyProperty<string> StrProperty { get; }
}