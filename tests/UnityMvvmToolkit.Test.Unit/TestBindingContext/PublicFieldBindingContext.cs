using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Global

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class PublicFieldBindingContext : IBindingContext
{
    public PublicFieldBindingContext()
    {
        boolField = new Property<bool>();
        _intField = new ReadOnlyProperty<int>(0);

        m_floatField = new Property<float>();
        StrField = new ReadOnlyProperty<string>(nameof(PublicFieldBindingContext));

        ObservablePublicField = new Property<bool>();
    }

    public IProperty<bool> boolField;
    public IReadOnlyProperty<int> _intField;
    public Property<float> m_floatField;
    public ReadOnlyProperty<string> StrField;

    [Observable("ObservableField")]
    public IProperty<bool> ObservablePublicField;
}