using System;
using System.Collections.Generic;
using System.Reflection;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Common.Properties;
using UnityMvvmToolkit.Common.ValueConverters;

namespace UnityMvvmToolkit.Common
{
    public class PropertyProvider<TBindingContext> : IPropertyProvider
    {
        private readonly TBindingContext _bindingContext;
        private readonly Dictionary<Type, IValueConverter> _valueConverters;

        public PropertyProvider(TBindingContext bindingContext)
        {
            _bindingContext = bindingContext;
            _valueConverters = new Dictionary<Type, IValueConverter>
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

            AssurePropertyExist(propertyName, out var propertyInfo);

            if (typeof(TCommand) != propertyInfo.PropertyType)
            {
                throw new InvalidCastException(
                    $"Can not cast the {propertyInfo.PropertyType} command to the {typeof(TCommand)} command.");
            }

            return (TCommand) propertyInfo.GetValue(_bindingContext);
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

            return (TProperty) Activator.CreateInstance(genericPropertyType, args);
        }

        private void AssurePropertyExist(string propertyName, out PropertyInfo propertyInfo)
        {
            propertyInfo = _bindingContext.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new NullReferenceException(nameof(propertyInfo));
            }
        }
    }
}