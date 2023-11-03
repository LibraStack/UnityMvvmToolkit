using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.ObjectWrappers
{
    internal abstract class ReadOnlyPropertyWrapper<TSource, TValue> : IReadOnlyProperty<TValue>, IPropertyWrapper
    {
        private int _converterId;

        private TValue _value;
        private TSource _sourceValue;
        private IReadOnlyProperty<TSource> _readOnlyProperty;

        [Preserve]
        protected ReadOnlyPropertyWrapper()
        {
            _converterId = -1;
        }

        public int ConverterId => _converterId;

        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value;
        }

        public event EventHandler<TValue> ValueChanged;

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
            if (_readOnlyProperty is not null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ReadOnlyPropertyWrapper<TSource, TValue>)} was not reset.");
            }

            _readOnlyProperty = (IReadOnlyProperty<TSource>)readOnlyProperty;
            _readOnlyProperty.ValueChanged += OnReadOnlyPropertyValueChanged;
            
            _sourceValue = _readOnlyProperty.Value;
            _value = Convert(_sourceValue);

            return this;
        }

        public void Reset()
        {
            _readOnlyProperty.ValueChanged -= OnReadOnlyPropertyValueChanged;
            _readOnlyProperty = null;
            
            _value = default;
        }
        
        private void OnReadOnlyPropertyValueChanged(object sender, TSource sourceValue)
        {
            if (EqualityComparer<TSource>.Default.Equals(_sourceValue, sourceValue) == false)
            {
                _sourceValue = sourceValue;
                _value = Convert(sourceValue);
            }

            ValueChanged?.Invoke(this, _value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract TValue Convert(TSource value);
    }
}