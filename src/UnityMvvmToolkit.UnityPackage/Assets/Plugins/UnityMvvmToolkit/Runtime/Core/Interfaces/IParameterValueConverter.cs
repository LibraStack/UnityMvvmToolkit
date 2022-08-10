using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IParameterValueConverter : IValueConverter
    {
        Type TargetType { get; }
    }

    public interface IParameterValueConverter<out TTargetType> : IParameterValueConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TTargetType Convert(ReadOnlyMemory<char> parameter);
    }
}