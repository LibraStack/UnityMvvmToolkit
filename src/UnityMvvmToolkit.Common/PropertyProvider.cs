using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Common.Properties;

namespace UnityMvvmToolkit.Common
{
    public class PropertyProvider<TBindingContext> : IPropertyProvider
    {
        private readonly TBindingContext _bindingContext;
        private readonly Dictionary<(string, Type), object> _cachedProperties;
        private readonly IReadOnlyDictionary<Type, IValueConverter> _valueConverters;

        public PropertyProvider(TBindingContext bindingContext,
            IReadOnlyDictionary<Type, IValueConverter> valueConverters)
        {
            _bindingContext = bindingContext;
            _cachedProperties = new Dictionary<(string, Type), object>();
            _valueConverters = valueConverters;
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
                args = new object[]
                    { _bindingContext, propertyInfo, GetValueConverter<TValueType>(propertyInfo.PropertyType) };
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
        private IValueConverter GetValueConverter<TValueType>(Type propertyType)
        {
            if (_valueConverters == null)
            {
                throw new NullReferenceException(nameof(_valueConverters));
            }

            if (_valueConverters.TryGetValue(propertyType, out var valueConverter))
            {
                return valueConverter;
            }

            throw new InvalidOperationException($"Converter is missing: From {propertyType} To {typeof(TValueType)}");
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