using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IValueConverter : IConverter
    {
        Type SourceType { get; }
        Type TargetType { get; }
    }

    public interface IValueConverter<TSourceType, TTargetType> : IValueConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TTargetType Convert(TSourceType value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TSourceType ConvertBack(TTargetType value);
    }
}