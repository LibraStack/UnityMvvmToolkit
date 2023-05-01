using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Local

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class ObservableFieldWithSameNameBindingContext : IBindingContext
{
    [Observable]
    private IProperty<bool> _bool;

    [Observable("Bool")]
    private IProperty<bool> m_bool;

    public ObservableFieldWithSameNameBindingContext()
    {
        _bool = new Property<bool>();
        m_bool = new Property<bool>();
    }
}