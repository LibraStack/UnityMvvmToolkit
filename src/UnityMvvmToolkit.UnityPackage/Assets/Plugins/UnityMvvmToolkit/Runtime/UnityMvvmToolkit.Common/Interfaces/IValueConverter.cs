using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IValueConverter
    {
    }

    public interface IValueConverter<TSourceType, TTargetType> : IValueConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryConvert(TSourceType value, out TTargetType result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryConvertBack(TTargetType value, out TSourceType result);
    }
}