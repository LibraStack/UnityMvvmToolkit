using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable UnusedMember.Local

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class SameFieldNameBindingContext1 : IBindingContext
{
    [Observable("Name")]
    private IProperty<int> _name = new Property<int>();
}

public class SameFieldNameBindingContext2 : IBindingContext
{
    [Observable("Name")]
    private IProperty<int> _name = new Property<int>();
}