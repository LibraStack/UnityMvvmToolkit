using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IPropertyValueConverter<TSource, TTarget> : IPropertyValueConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TTarget Convert(TSource value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TSource ConvertBack(TTarget value);
    }
}