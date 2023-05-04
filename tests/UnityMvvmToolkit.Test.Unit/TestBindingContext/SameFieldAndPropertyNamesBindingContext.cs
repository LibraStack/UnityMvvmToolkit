using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class SameFieldAndPropertyNamesBindingContext : IBindingContext
{
    private readonly IProperty<int> _count = new Property<int>();

    public IReadOnlyProperty<int> Count => _count;
}