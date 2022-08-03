using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Common.Properties;
using UnityMvvmToolkit.Common.ValueConverters;

namespace UnityMvvmToolkit.Common
{
    public class PropertyProvider<TBindingContext> : IPropertyProvider
    {
        private readonly TBindingContext _bindingContext;
        private readonly Dictionary<Type, IValueConverter> _valueConverters;
        private readonly Dictionary<(string, Type), object> _cachedProperties;

        public PropertyProvider(TBindingContext bindingContext)
        {
            _bindingContext = bindingContext;
            _cachedProperties = new Dictionary<(string, Type), object>();

            _valueConverters = new Dictionary<Type, IValueConverter> // TODO: Register user converters.
            {
                { typeof(int), new IntToStrConverter() },
                { typeof(float), new FloatToStrConverter() }
            };
        }

        public TCommand GetCommand<TCommand>(string propertyName) where TCommand : IBaseCommand
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return default;
            }

            if (TryGetPropertyFromCache<TCommand>(propertyName, out var command))
            {
                return command;
            }

            AssurePropertyExist(propertyName, out var propertyInfo);

            if (typeof(TCommand) != propertyInfo.PropertyType)
            {
                throw new InvalidCastException(
                    $"Can not cast the {propertyInfo.PropertyType} command to the {typeof(TCommand)} command.");
            }

            return CacheProperty<TCommand>(propertyName, propertyInfo.GetValue(_bindingContext));
        }

        public IProperty<TValueType> GetProperty<TValueType>(string propertyName)
        {
            return CreateProperty<IProperty<TValueType>, TValueType>(propertyName, typeof(Property<,>),
                typeof(PropertyWithValueConverter<,,>));
        }

        public IReadOnlyProperty<TValueType> GetReadOnlyProperty<TValueType>(string propertyName)
        {
            return CreateProperty<IReadOnlyProperty<TValueType>, TValueType>(propertyName, typeof(ReadOnlyProperty<,>),
                typeof(ReadOnlyPropertyWithValueConverter<,,>));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TProperty CreateProperty<TProperty, TValueType>(string propertyName, Type propertyType,
            Type propertyWithValueConverterType)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return default;
            }

            if (typeof(TValueType) == typeof(IBaseCommand))
            {
                throw new InvalidOperationException($"To get a command use the {nameof(GetCommand)} method instead.");
            }

            if (TryGetPropertyFromCache<TProperty>(propertyName, out var property))
            {
                return property;
            }

            AssurePropertyExist(propertyName, out var propertyInfo);

            object[] args;
            Type genericPropertyType;

            if (typeof(TValueType) == propertyInfo.PropertyType)
            {
                args = new object[] { _bindingContext, propertyInfo };
                genericPropertyType = propertyType.MakeGenericType(typeof(TBindingContext), typeof(TValueType));
            }
            else
            {
                args = new object[] { _bindingContext, propertyInfo, _valueConverters[propertyInfo.PropertyType] };
                genericPropertyType = propertyWithValueConverterType.MakeGenericType(typeof(TBindingContext),
                    typeof(TValueType), propertyInfo.PropertyType);
            }

            return CacheProperty<TProperty>(propertyName, Activator.CreateInstance(genericPropertyType, args));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssurePropertyExist(string propertyName, out PropertyInfo propertyInfo)
        {
            propertyInfo = _bindingContext.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new NullReferenceException(nameof(propertyInfo));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T CacheProperty<T>(string propertyName, object property)
        {
            _cachedProperties.Add((propertyName, typeof(T)), property);
            return (T) property;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetPropertyFromCache<T>(string propertyName, out T property)
        {
            if (_cachedProperties.TryGetValue((propertyName, typeof(T)), out var cachedProperty))
            {
                property = (T) cachedProperty;
                return true;
            }

            property = default;
            return false;
        }
    }
}