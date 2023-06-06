using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IProperty<T> : IReadOnlyProperty<T>, IProperty
    {
        new T Value { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TrySetValue(T value);

        internal void ForceSetValue(T value);
    }
}