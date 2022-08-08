using System;
using System.Collections.Generic;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Common.Internal.StringParsers;

namespace UnityMvvmToolkit.Common
{
    public abstract class BindablePropertyElement : BindableCommandElement, IBindablePropertyElement
    {
        private readonly List<string> _bindableProperties;
        private readonly IObjectProvider _objectProvider;
        private readonly PropertyStringParser _propertyStringParser;

        protected BindablePropertyElement(IObjectProvider objectProvider) : base(objectProvider)
        {
            _objectProvider = objectProvider;
            _bindableProperties = new List<string>();
            _propertyStringParser = new PropertyStringParser();
        }

        public IEnumerable<string> BindableProperties => _bindableProperties;

        public abstract void UpdateValues();

        protected IProperty<TValueType> GetProperty<TValueType>(string propertyStringData)
        {
            var bindingData = _propertyStringParser.GetPropertyData(propertyStringData.AsMemory());
            var propertyName = bindingData.PropertyName.ToString();

            var property = _objectProvider.GetProperty<TValueType>(propertyName, bindingData.ConverterName);
            if (property != null)
            {
                _bindableProperties.Add(propertyName);
            }

            return property;
        }

        protected IReadOnlyProperty<TValueType> GetReadOnlyProperty<TValueType>(string bindingStringData)
        {
            var bindingData = _propertyStringParser.GetPropertyData(bindingStringData.AsMemory());
            var propertyName = bindingData.PropertyName.ToString();

            var property = _objectProvider.GetReadOnlyProperty<TValueType>(propertyName, bindingData.ConverterName);
            if (property != null)
            {
                _bindableProperties.Add(propertyName);
            }

            return property;
        }
    }
}