using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.Internal.BindingContextObjectWrappers.PropertyWrappers
{
    internal class PropertyWrapper<TObjectType, TValueType> : IProperty<TValueType>
    {
        private readonly TObjectType _obj;
        private readonly Func<TObjectType, TValueType> _getPropertyDelegate;
        private readonly Action<TObjectType, TValueType> _setPropertyDelegate;

        public PropertyWrapper(TObjectType obj, PropertyInfo propertyInfo)
        {
            _obj = obj;
            _getPropertyDelegate = propertyInfo.CreateGetValueDelegate<TObjectType, TValueType>();
            _setPropertyDelegate = propertyInfo.CreateSetValueDelegate<TObjectType, TValueType>();
        }

        public TValueType Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _getPropertyDelegate(_obj);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _setPropertyDelegate(_obj, value);
        }
    }
}