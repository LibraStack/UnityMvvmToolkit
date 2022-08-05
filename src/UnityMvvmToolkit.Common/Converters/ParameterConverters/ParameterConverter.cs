using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.Converters.ParameterConverters
{
    public abstract class ParameterConverter<TTargetType> : IParameterConverter<TTargetType>
    {
        protected ParameterConverter()
        {
            Name = GetType().Name;
            TargetType = typeof(TTargetType);
        }

        public string Name { get; }
        public Type TargetType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract bool TryConvert(ReadOnlyMemory<char> parameter, out TTargetType result);
    }
}