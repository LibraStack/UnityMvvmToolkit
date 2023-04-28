using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class PublicPropertyBindingContext : IBindingContext
{
    public PublicPropertyBindingContext()
    {
        Bool = new Property<bool>();
        Int = new ReadOnlyProperty<int>(0);

        Float = new Property<float>();
        Str = new ReadOnlyProperty<string>(nameof(PrivateFieldBindingContext));
    }

    public IProperty<bool> Bool { get; }
    public IReadOnlyProperty<int> Int { get; }
    public Property<float> Float { get; }
    public ReadOnlyProperty<string> Str { get; }
}