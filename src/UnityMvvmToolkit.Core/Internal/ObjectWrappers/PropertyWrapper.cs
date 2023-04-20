using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.ObjectWrappers
{
    internal class PropertyWrapper<TValueType, TSourceType> : IProperty<TValueType>, IPropertyWrapper
    {
        private readonly IPropertyValueConverter<TSourceType, TValueType> _valueConverter;

        private TValueType _value;
        private TSourceType _sourceValue;
        private IProperty<TSourceType> _property;

        public PropertyWrapper(IPropertyValueConverter<TSourceType, TValueType> valueConverter)
        {
            _valueConverter = valueConverter;
        }

        public int ConverterId { get; private set; }

        public TValueType Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => TrySetValue(value);
        }

        public event EventHandler<TValueType> ValueChanged;

        public IPropertyWrapper SetConverterId(int converterId)
        {
            ConverterId = converterId;
            return this;
        }

        public IPropertyWrapper SetProperty(object property)
        {
            if (_property != null)
            {
                throw new InvalidOperationException(
                    $"{nameof(PropertyWrapper<TValueType, TSourceType>)} was not reset.");
            }

            _property = (IProperty<TSourceType>) property;
            _property.ValueChanged += OnPropertyValueChanged;

            _sourceValue = _property.Value;
            _value = _valueConverter.Convert(_sourceValue);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetValue(TValueType value)
        {
            if (EqualityComparer<TValueType>.Default.Equals(_value, value))
            {
                return false;
            }

            _value = value;

            _sourceValue = _valueConverter.ConvertBack(value);
            _property.ForceSetValue(_sourceValue);

            return true;
        }

        public void Reset()
        {
            _property.ValueChanged -= OnPropertyValueChanged;
            _property = null;

            _value = default;
        }

        private void OnPropertyValueChanged(object sender, TSourceType sourceValue)
        {
            if (EqualityComparer<TSourceType>.Default.Equals(_sourceValue, sourceValue) == false)
            {
                _sourceValue = sourceValue;
                _value = _valueConverter.Convert(sourceValue);
            }

            ValueChanged?.Invoke(this, _value);
        }

        void IProperty<TValueType>.ForceSetValue(TValueType value)
        {
            throw new NotImplementedException();
        }
    }
}