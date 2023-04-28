using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Test.Unit.TestBindingContext;

public class PublicFieldBindingContext : IBindingContext
{
    public PublicFieldBindingContext()
    {
        Bool = new Property<bool>();
        Int = new ReadOnlyProperty<int>(0);

        Float = new Property<float>();
        Str = new ReadOnlyProperty<string>(nameof(PrivateFieldBindingContext));
    }

    public IProperty<bool> Bool;
    public IReadOnlyProperty<int> Int;
    public Property<float> Float;
    public ReadOnlyProperty<string> Str;
}