using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.PropertyWrappers
{
    internal class PropertyWithConverter<TValueType, TSourceType> : IProperty<TValueType>, IPropertyWrapper
    {
        private readonly int _hashCode;
        private readonly IPropertyValueConverter<TSourceType, TValueType> _valueConverter;

        private TValueType _value;
        private TSourceType _sourceValue;
        private IProperty<TSourceType> _property;

        public PropertyWithConverter(IPropertyValueConverter<TSourceType, TValueType> valueConverter)
        {
            _hashCode = IPropertyWrapper.GenerateHashCode(typeof(TValueType), typeof(TSourceType));
            _valueConverter = valueConverter;
        }

        public IPropertyWrapper SetProperty(object property)
        {
            if (_property != null)
            {
                throw new InvalidOperationException("PropertyWithConverter was not reset.");
            }

            _property = (IProperty<TSourceType>) property;
            _property.ValueChanged += OnPropertyValueChanged;

            _sourceValue = _property.Value;
            _value = _valueConverter.Convert(_sourceValue);

            return this;
        }

        public void Reset()
        {
            _property.ValueChanged -= OnPropertyValueChanged;
            _property = null;

            _value = default;
        }

        public TValueType Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => TrySetValue(value);
        }

        public event EventHandler<TValueType> ValueChanged;

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

        public override int GetHashCode()
        {
            return _hashCode;
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