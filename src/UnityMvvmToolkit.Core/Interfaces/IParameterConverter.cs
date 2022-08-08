using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IParameterConverter : IConverter
    {
        Type TargetType { get; }
    }

    public interface IParameterConverter<out TTargetType> : IParameterConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TTargetType Convert(ReadOnlyMemory<char> parameter);
    }
}