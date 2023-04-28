using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable NotAccessedField.Local

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class ObservableFieldWithSameNameBindingContext : IBindingContext
{
    [Observable]
    private IProperty<bool> _bool;

    [Observable("Bool")]
    private IProperty<bool> _boolWithPropertyName;

    public ObservableFieldWithSameNameBindingContext()
    {
        _bool = new Property<bool>();
        _boolWithPropertyName = new Property<bool>();
    }
}