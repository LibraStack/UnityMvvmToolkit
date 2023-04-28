using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable NotAccessedField.Local

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class ObservableFieldBindingContext : IBindingContext
{
    [Observable]
    private IProperty<bool> _bool;

    [Observable("BoolName")]
    private IProperty<bool> _boolWithPropertyName;

    public ObservableFieldBindingContext()
    {
        _bool = new Property<bool>();
        _boolWithPropertyName = new Property<bool>();
    }
}