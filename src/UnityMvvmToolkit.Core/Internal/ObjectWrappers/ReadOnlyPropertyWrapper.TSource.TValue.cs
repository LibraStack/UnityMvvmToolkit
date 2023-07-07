using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.ObjectWrappers
{
    internal sealed class ReadOnlyPropertyWrapper<TSource, TValue> : IReadOnlyProperty<TValue>, IPropertyWrapper
    {
        private readonly IPropertyValueConverter<TSource, TValue> _valueConverter;

        private int _converterId;
        private bool _isInitialized;

        private TValue _value;

        [Preserve]
        public ReadOnlyPropertyWrapper(IPropertyValueConverter<TSource, TValue> valueConverter)
        {
            _converterId = -1;
            _valueConverter = valueConverter;
        }

        public int ConverterId => _converterId;

        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value;
        }

        #pragma warning disable 67
        public event EventHandler<TValue> ValueChanged;
        #pragma warning restore 67

        public IPropertyWrapper SetConverterId(int converterId)
        {
            if (_converterId != -1)
            {
                throw new InvalidOperationException("Can not change converter ID.");
            }

            _converterId = converterId;

            return this;
        }

        public IPropertyWrapper SetProperty(IBaseProperty readOnlyProperty)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException(
                    $"{nameof(ReadOnlyPropertyWrapper<TValue, TSource>)} was not reset.");
            }

            _value = _valueConverter.Convert(((IReadOnlyProperty<TSource>) readOnlyProperty).Value);
            _isInitialized = true;

            return this;
        }

        public void Reset()
        {
            _value = default;
            _isInitialized = false;
        }
    }
}