using System.Collections.Generic;
using System.ComponentModel;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Common.Internal;

namespace UnityMvvmToolkit.Common
{
    public class View<TBindingContext> where TBindingContext : class, INotifyPropertyChanged
    {
        private TBindingContext _bindingContext;

        private IObjectProvider _objectProvider;
        private IBindableElementsWrapper _bindableElementsWrapper;
        private Dictionary<string, HashSet<IBindablePropertyElement>> _bindableVisualElements;

        public TBindingContext BindingContext => _bindingContext;

        public void Configure(TBindingContext bindingContext, IBindableElementsWrapper elementsWrapper,
            IConverter[] converters)
        {
            _bindingContext = bindingContext;
            _objectProvider = new BindingContextObjectProvider<TBindingContext>(bindingContext, converters);
            _bindableElementsWrapper = elementsWrapper;
            _bindableVisualElements = new Dictionary<string, HashSet<IBindablePropertyElement>>();
        }

        public void EnableBinding()
        {
            _bindingContext.PropertyChanged += OnBindingContextPropertyChanged;
        }

        public void DisableBinding()
        {
            _bindingContext.PropertyChanged -= OnBindingContextPropertyChanged;
        }

        public IBindableElement RegisterBindableElement(IBindableUIElement bindableUiElement, bool updateElementValues)
        {
            var bindableElement = _bindableElementsWrapper.Wrap(bindableUiElement, _objectProvider);
            if (bindableElement is not IBindablePropertyElement bindableVisualElement)
            {
                return bindableElement;
            }

            foreach (var propertyName in bindableVisualElement.BindableProperties)
            {
                RegisterBindableElement(propertyName, bindableVisualElement);
            }

            if (updateElementValues)
            {
                bindableVisualElement.UpdateValues();
            }

            return bindableElement;
        }

        private void RegisterBindableElement(string propertyName, IBindablePropertyElement bindablePropertyElement)
        {
            if (_bindableVisualElements.TryGetValue(propertyName, out var visualElements))
            {
                visualElements.Add(bindablePropertyElement);
            }
            else
            {
                _bindableVisualElements.Add(propertyName,
                    new HashSet<IBindablePropertyElement> { bindablePropertyElement });
            }
        }

        private void OnBindingContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_bindableVisualElements.TryGetValue(e.PropertyName, out var visualElements))
            {
                foreach (var visualElement in visualElements)
                {
                    visualElement.UpdateValues();
                }
            }
        }
    }
}