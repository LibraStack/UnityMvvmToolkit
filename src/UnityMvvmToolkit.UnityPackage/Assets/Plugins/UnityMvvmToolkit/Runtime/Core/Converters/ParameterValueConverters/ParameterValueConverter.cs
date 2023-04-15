using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Converters.ParameterValueConverters
{
    public abstract class ParameterValueConverter<TTargetType> : IParameterValueConverter<TTargetType>
    {
        protected ParameterValueConverter()
        {
            Name = GetType().Name;
            TargetType = typeof(TTargetType);
        }

        public string Name { get; }
        public Type TargetType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract TTargetType Convert(string parameter);
    }
}