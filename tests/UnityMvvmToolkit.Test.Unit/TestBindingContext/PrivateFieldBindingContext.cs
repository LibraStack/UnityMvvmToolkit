using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Local

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class PrivateFieldBindingContext : IBindingContext
{
    private IProperty<bool> boolField;
    private IReadOnlyProperty<int> _intField;

    private Property<float> m_floatField;
    private ReadOnlyProperty<string> StrField;

    public PrivateFieldBindingContext()
    {
        boolField = new Property<bool>();
        _intField = new ReadOnlyProperty<int>(0);

        m_floatField = new Property<float>();
        StrField = new ReadOnlyProperty<string>(nameof(PrivateFieldBindingContext));
    }
}