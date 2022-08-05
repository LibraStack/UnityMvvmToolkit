using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IParameterConverter : IConverter
    {
        Type TargetType { get; }
    }

    public interface IParameterConverter<TTargetType> : IParameterConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryConvert(ReadOnlyMemory<char> parameter, out TTargetType result);
    }
}