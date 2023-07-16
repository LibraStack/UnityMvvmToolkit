using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Internal.ObjectWrappers
{
    internal sealed class PropertyCastWrapper<TSource, TValue> : PropertyWrapper<TSource, TValue>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override TValue Convert(TSource value)
        {
            return (TValue) (object) value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override TSource ConvertBack(TValue value)
        {
            return (TSource) (object) value;
        }
    }
}