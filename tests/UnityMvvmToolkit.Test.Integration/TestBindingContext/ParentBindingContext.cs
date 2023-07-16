using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Test.Integration.TestBindingContext;

public class ParentBindingContext : IBindingContext
{
    public ParentBindingContext()
    {
        FloatPropertyBindingContext = new Property<float>();
        NestedProperty = new Property<NestedClass>();

        NestedPropertyBindingContext = new Property<NestedBindingContext>(new NestedBindingContext());
        NestedReadOnlyPropertyBindingContext = new ReadOnlyProperty<NestedBindingContext>(new NestedBindingContext());
    }

    public IProperty<float> FloatPropertyBindingContext { get; }
    public IProperty<NestedClass> NestedProperty { get; }
    public IProperty<NestedBindingContext> NestedPropertyBindingContext { get; }
    public IReadOnlyProperty<NestedBindingContext> NestedReadOnlyPropertyBindingContext { get; }
}