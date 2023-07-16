using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Internal.ObjectWrappers
{
    internal sealed class ReadOnlyPropertyCastWrapper<TSource, TValue> : ReadOnlyPropertyWrapper<TSource, TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override TValue Convert(TSource value)
        {
            return (TValue) (object) value;
        }
    }
}