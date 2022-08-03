using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.Properties
{
    public class PropertyWithValueConverter<TObjectType, TValueType, TSourceType> : IProperty<TValueType>
    {
        private readonly TObjectType _obj;
        private readonly Func<TObjectType, TSourceType> _getPropertyDelegate;
        private readonly Action<TObjectType, TSourceType> _setPropertyDelegate;
        private readonly IValueConverter<TSourceType, TValueType> _valueConverter;

        public PropertyWithValueConverter(TObjectType obj, PropertyInfo propertyInfo,
            IValueConverter<TSourceType, TValueType> valueConverter)
        {
            _obj = obj;
            _valueConverter = valueConverter;
            _getPropertyDelegate = propertyInfo.CreateGetValueDelegate<TObjectType, TSourceType>();
            _setPropertyDelegate = propertyInfo.CreateSetValueDelegate<TObjectType, TSourceType>();
        }

        public TValueType Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _valueConverter.TryConvert(_getPropertyDelegate(_obj), out var result) ? result : default;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (_valueConverter.TryConvertBack(value, out var propertyValue))
                {
                    _setPropertyDelegate(_obj, propertyValue);
                }
            }
        }
    }
}