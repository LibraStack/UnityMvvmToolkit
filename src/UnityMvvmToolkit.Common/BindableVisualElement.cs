using System.Collections.Generic;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common
{
    public abstract class BindableVisualElement : IBindableVisualElement
    {
        private readonly IPropertyProvider _propertyProvider;
        private readonly List<string> _bindableProperties;

        protected BindableVisualElement(IPropertyProvider propertyProvider)
        {
            _propertyProvider = propertyProvider;
            _bindableProperties = new List<string>();
        }

        public IEnumerable<string> BindableProperties => _bindableProperties;

        public abstract void UpdateValues();

        protected IProperty<TValueType> GetProperty<TValueType>(string propertyName)
        {
            var property = _propertyProvider.GetProperty<TValueType>(propertyName);
            if (property != null && IsNotCommand<TValueType>())
            {
                _bindableProperties.Add(propertyName);
            }

            return property;
        }

        protected IReadOnlyProperty<TValueType> GetReadOnlyProperty<TValueType>(string propertyName)
        {
            var property = _propertyProvider.GetReadOnlyProperty<TValueType>(propertyName);
            if (property != null && IsNotCommand<TValueType>())
            {
                _bindableProperties.Add(propertyName);
            }

            return property;
        }

        private bool IsNotCommand<T>()
        {
            return typeof(T) != typeof(ICommand);
        }
    }
}