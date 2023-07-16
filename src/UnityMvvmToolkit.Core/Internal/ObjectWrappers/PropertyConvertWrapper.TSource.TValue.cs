using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.ObjectWrappers
{
    internal sealed class PropertyConvertWrapper<TSource, TValue> : PropertyWrapper<TSource, TValue>
    {
        private readonly IPropertyValueConverter<TSource, TValue> _valueConverter;

        [Preserve]
        public PropertyConvertWrapper(IPropertyValueConverter<TSource, TValue> valueConverter)
        {
            _valueConverter = valueConverter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override TValue Convert(TSource value)
        {
            return _valueConverter.Convert(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override TSource ConvertBack(TValue value)
        {
            return _valueConverter.ConvertBack(value);
        }
    }
}