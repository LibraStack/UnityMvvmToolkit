using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class InvalidFieldNameBindingContext : IBindingContext
{
    [Observable]
    private IProperty<int> _ = new Property<int>();

    [Observable]
    private IProperty<int> m_ = new Property<int>();
}