using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class InvalidFieldNameBindingContext : IBindingContext
{
    private IProperty<int> _ = new Property<int>();
    private IProperty<int> m_ = new Property<int>();
}