using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class ObservablePropertyBindingContext : IBindingContext
{
    [Observable]
    private IProperty<bool> _bool { get; }

    [Observable("BoolName")]
    protected IProperty<bool> m_boolWithPropertyName { get; }

    [Observable]
    public string? StrProperty { get; set; }

    public ObservablePropertyBindingContext()
    {
        _bool = new Property<bool>();
        m_boolWithPropertyName = new Property<bool>();
    }
}