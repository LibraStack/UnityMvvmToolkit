using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class ObservablePropertyBindingContext : IBindingContext
{
    [Observable]
    private IProperty<bool> _bool { get; }

    [Observable("BoolName")]
    protected IProperty<bool> m_boolWithPropertyName { get; }

    public ObservablePropertyBindingContext()
    {
        _bool = new Property<bool>();
        m_boolWithPropertyName = new Property<bool>();
    }
}