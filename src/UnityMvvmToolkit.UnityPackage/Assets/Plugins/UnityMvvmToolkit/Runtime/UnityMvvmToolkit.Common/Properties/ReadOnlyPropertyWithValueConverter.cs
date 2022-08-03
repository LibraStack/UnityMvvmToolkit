using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.Properties
{
    public class ReadOnlyPropertyWithValueConverter<TObjectType, TValueType, TSourceType> : IReadOnlyProperty<TValueType>
    {
        private readonly TObjectType _obj;
        private readonly Func<TObjectType, TSourceType> _getPropertyDelegate;
        private readonly IValueConverter<TSourceType, TValueType> _valueConverter;

        public ReadOnlyPropertyWithValueConverter(TObjectType obj, PropertyInfo propertyInfo,
            IValueConverter<TSourceType, TValueType> valueConverter)
        {
            _obj = obj;
            _valueConverter = valueConverter;
            _getPropertyDelegate = propertyInfo.CreateGetValueDelegate<TObjectType, TSourceType>();
        }

        public TValueType Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _valueConverter.TryConvert(_getPropertyDelegate(_obj), out var result) ? result : default;
        }
    }
}