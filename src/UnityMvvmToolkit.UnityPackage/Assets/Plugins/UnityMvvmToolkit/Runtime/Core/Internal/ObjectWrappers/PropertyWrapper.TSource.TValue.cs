using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.ObjectWrappers
{
    internal abstract class PropertyWrapper<TSource, TValue> : IProperty<TValue>, IPropertyWrapper
    {
        private int _converterId;

        private TValue _value;
        private TSource _sourceValue;
        private IProperty<TSource> _property;

        [Preserve]
        protected PropertyWrapper()
        {
            _converterId = -1;
        }

        public int ConverterId => _converterId;

        public TValue Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => TrySetValue(value);
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

        public IPropertyWrapper SetProperty(IBaseProperty property)
        {
            if (_property is not null)
            {
                throw new InvalidOperationException(
                    $"{nameof(PropertyWrapper<TSource, TValue>)} was not reset.");
            }

            _property = (IProperty<TSource>) property;
            _property.ValueChanged += OnPropertyValueChanged;

            _sourceValue = _property.Value;
            _value = Convert(_sourceValue);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetValue(TValue value)
        {
            if (EqualityComparer<TValue>.Default.Equals(_value, value))
            {
                return false;
            }

            _value = value;

            _sourceValue = ConvertBack(value);
            _property.ForceSetValue(_sourceValue);

            return true;
        }

        public void Reset()
        {
            _property.ValueChanged -= OnPropertyValueChanged;
            _property = null;

            _value = default;
        }

        private void OnPropertyValueChanged(object sender, TSource sourceValue)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract TSource ConvertBack(TValue value);

        void IProperty<TValue>.ForceSetValue(TValue value) => throw new NotImplementedException();
    }
}