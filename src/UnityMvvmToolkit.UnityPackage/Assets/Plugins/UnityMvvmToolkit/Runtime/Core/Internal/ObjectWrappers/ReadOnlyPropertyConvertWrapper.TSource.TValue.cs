using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.ObjectWrappers
{
    internal sealed class ReadOnlyPropertyConvertWrapper<TSource, TValue> : ReadOnlyPropertyWrapper<TSource, TValue>
    {
        private readonly IPropertyValueConverter<TSource, TValue> _valueConverter;

        [Preserve]
        public ReadOnlyPropertyConvertWrapper(IPropertyValueConverter<TSource, TValue> valueConverter)
        {
            _valueConverter = valueConverter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override TValue Convert(TSource value)
        {
            return _valueConverter.Convert(value);
        }
    }
}