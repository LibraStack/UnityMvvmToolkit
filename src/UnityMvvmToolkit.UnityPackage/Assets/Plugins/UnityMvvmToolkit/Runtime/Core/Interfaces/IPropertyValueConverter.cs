using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IPropertyValueConverter : IValueConverter
    {
        Type SourceType { get; }
        Type TargetType { get; }
    }

    public interface IPropertyValueConverter<TSourceType, TTargetType> : IPropertyValueConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TTargetType Convert(TSourceType value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TSourceType ConvertBack(TTargetType value);
    }
}