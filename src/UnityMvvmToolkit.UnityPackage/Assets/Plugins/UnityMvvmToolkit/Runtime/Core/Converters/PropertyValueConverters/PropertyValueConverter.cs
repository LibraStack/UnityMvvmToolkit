using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Converters.PropertyValueConverters
{
    public abstract class PropertyValueConverter<TSourceType, TTargetType> 
        : IPropertyValueConverter<TSourceType, TTargetType>
    {
        protected PropertyValueConverter()
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