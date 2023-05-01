using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable InconsistentNaming

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class PublicFieldBindingContext : IBindingContext
{
    public PublicFieldBindingContext()
    {
        boolField = new Property<bool>();
        _intField = new ReadOnlyProperty<int>(0);

        m_floatField = new Property<float>();
        StrField = new ReadOnlyProperty<string>(nameof(PublicFieldBindingContext));
    }

    public IProperty<bool> boolField;
    public IReadOnlyProperty<int> _intField;
    public Property<float> m_floatField;
    public ReadOnlyProperty<string> StrField;
}