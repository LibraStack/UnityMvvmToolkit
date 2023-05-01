using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Local

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class ObservableFieldBindingContext : IBindingContext
{
    [Observable]
    private IProperty<bool> _bool;

    [Observable("BoolName")]
    private IProperty<bool> m_boolWithPropertyName;

    public ObservableFieldBindingContext()
    {
        _bool = new Property<bool>();
        m_boolWithPropertyName = new Property<bool>();
    }
}