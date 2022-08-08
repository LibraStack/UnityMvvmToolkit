using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Converters.ParameterConverters
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
        public abstract TTargetType Convert(ReadOnlyMemory<char> parameter);
    }
}