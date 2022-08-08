using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.Converters.ValueConverters
{
    public abstract class ValueConverter<TSourceType, TTargetType> : IValueConverter<TSourceType, TTargetType>
    {
        protected ValueConverter()
        {
            Name = GetType().Name;
            SourceType = typeof(TSourceType);
            TargetType = typeof(TTargetType);
        }

        public string Name { get; }
        public Type SourceType { get; }
        public Type TargetType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract TTargetType Convert(TSourceType value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract TSourceType ConvertBack(TTargetType value);
    }
}