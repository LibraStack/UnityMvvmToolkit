using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.Internal.BindingContextObjectWrappers.PropertyWrappers
{
    internal class ReadOnlyPropertyWrapper<TObjectType, TValueType> : IReadOnlyProperty<TValueType>
    {
        private readonly TObjectType _obj;
        private readonly Func<TObjectType, TValueType> _getPropertyDelegate;

        public ReadOnlyPropertyWrapper(TObjectType obj, PropertyInfo propertyInfo)
        {
            _obj = obj;
            _getPropertyDelegate = propertyInfo.CreateGetValueDelegate<TObjectType, TValueType>();
        }

        public TValueType Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _getPropertyDelegate(_obj);
        }
    }
}