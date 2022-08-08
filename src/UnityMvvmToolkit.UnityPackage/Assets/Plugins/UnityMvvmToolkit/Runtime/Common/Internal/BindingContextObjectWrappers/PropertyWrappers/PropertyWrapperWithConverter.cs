using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.Internal.BindingContextObjectWrappers.PropertyWrappers
{
    internal class PropertyWrapperWithConverter<TObjectType, TValueType, TSourceType> : IProperty<TValueType>
    {
        private readonly TObjectType _obj;
        private readonly Func<TObjectType, TSourceType> _getPropertyDelegate;
        private readonly Action<TObjectType, TSourceType> _setPropertyDelegate;
        private readonly IValueConverter<TSourceType, TValueType> _valueConverter;

        public PropertyWrapperWithConverter(TObjectType obj, PropertyInfo propertyInfo,
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
            get => _valueConverter.Convert(_getPropertyDelegate(_obj));
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _setPropertyDelegate(_obj, _valueConverter.ConvertBack(value));
        }
    }
}