using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common
{
    public class View<TBindingContext> where TBindingContext : class, INotifyPropertyChanged
    {
        private TBindingContext _bindingContext;

        private IBindableVisualElementsCreator _bindableElementsCreator;
        private Dictionary<string, HashSet<IBindableVisualElement>> _bindableVisualElements;

        public TBindingContext BindingContext => _bindingContext;

        public void Configure(TBindingContext bindingContext, IBindableVisualElementsCreator visualElementsCreator)
        {
            _bindingContext = bindingContext;
            _bindableElementsCreator = visualElementsCreator;
            _bindableVisualElements = new Dictionary<string, HashSet<IBindableVisualElement>>();
        }

        public void EnableBinding()
        {
            _bindingContext.PropertyChanged += OnBindingContextPropertyChanged;
        }

        public void DisableBinding()
        {
            _bindingContext.PropertyChanged -= OnBindingContextPropertyChanged;
        }

        public IBindableElement RegisterBindableElement(IBindableUIElement bindableUiElement, TBindingContext bindingContext)
        {
            var propertyInfo = bindingContext.GetType().GetProperty(bindableUiElement.BindablePropertyName);
            if (propertyInfo == null)
            {
                throw new NullReferenceException(nameof(propertyInfo));
            }

            var bindableElement = _bindableElementsCreator.Create(bindingContext, bindableUiElement, propertyInfo);
            if (bindableElement is IBindableVisualElement bindableVisualElement)
            {
                RegisterBindableElement(bindableVisualElement, bindableUiElement.BindablePropertyName);
            }

            return bindableElement;
        }

        private void RegisterBindableElement(IBindableVisualElement bindableElement, string propertyName)
        {
            if (_bindableVisualElements.TryGetValue(propertyName, out var visualElements) == false)
            {
                visualElements = new HashSet<IBindableVisualElement>();
                _bindableVisualElements.Add(propertyName, visualElements);
            }

            bindableElement.UpdateValue();
            visualElements.Add(bindableElement);
        }

        private void OnBindingContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_bindableVisualElements.TryGetValue(e.PropertyName, out var visualElements))
            {
                foreach (var visualElement in visualElements)
                {
                    visualElement.UpdateValue();
                }
            }
        }
    }
}