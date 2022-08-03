using System;
using System.Collections.Generic;
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

        public IProperty<TValueType> GetProperty<TValueType>(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            var propertyInfo = _bindingContext.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new NullReferenceException(nameof(propertyInfo));
            }

            if (typeof(TValueType) == propertyInfo.PropertyType)
            {
                return new Property<TBindingContext, TValueType>(_bindingContext, propertyInfo);
            }

            // TODO: Cache source properties.
            var genericPropertyType = typeof(PropertyWithValueConverter<,,>)
                .MakeGenericType(typeof(TBindingContext), typeof(TValueType), propertyInfo.PropertyType);

            var sourcePropertyInstance =
                Activator.CreateInstance(genericPropertyType, _bindingContext, propertyInfo,
                    _valueConverters[propertyInfo.PropertyType]);

            return (IProperty<TValueType>) sourcePropertyInstance;
        }

        public IReadOnlyProperty<TValueType> GetReadOnlyProperty<TValueType>(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            var propertyInfo = _bindingContext.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new NullReferenceException(nameof(propertyInfo));
            }

            if (typeof(TValueType) == propertyInfo.PropertyType)
            {
                return new ReadOnlyProperty<TBindingContext, TValueType>(_bindingContext, propertyInfo);
            }

            // TODO: Cache source properties.
            var genericPropertyType = typeof(ReadOnlyPropertyWithValueConverter<,,>)
                .MakeGenericType(typeof(TBindingContext), typeof(TValueType), propertyInfo.PropertyType);

            var sourcePropertyInstance =
                Activator.CreateInstance(genericPropertyType, _bindingContext, propertyInfo,
                    _valueConverters[propertyInfo.PropertyType]);

            return (IReadOnlyProperty<TValueType>) sourcePropertyInstance;
        }
    }
}