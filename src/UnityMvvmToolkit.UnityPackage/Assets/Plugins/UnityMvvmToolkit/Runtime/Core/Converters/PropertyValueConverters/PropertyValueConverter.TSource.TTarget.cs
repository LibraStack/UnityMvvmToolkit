using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Converters.PropertyValueConverters
{
    public abstract class PropertyValueConverter<TSource, TTarget> 
        : IPropertyValueConverter<TSource, TTarget>
    {
        protected PropertyValueConverter()
        {
            Name = GetType().Name;
            SourceType = typeof(TSource);
            TargetType = typeof(TTarget);
        }

        public string Name { get; }
        public Type SourceType { get; }
        public Type TargetType { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract TTarget Convert(TSource value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract TSource ConvertBack(TTarget value);
    }
}