using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IProperty<TType> : IReadOnlyProperty<TType>
    {
        new TType Value { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TrySetValue(TType value);

        internal void ForceSetValue(TType value);
    }
}