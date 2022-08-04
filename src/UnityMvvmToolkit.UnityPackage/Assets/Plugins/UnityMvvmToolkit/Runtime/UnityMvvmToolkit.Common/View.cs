using System.Collections.Generic;
using System.ComponentModel;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common
{
    public class View<TBindingContext> where TBindingContext : class, INotifyPropertyChanged
    {
        private TBindingContext _bindingContext;

        private IPropertyProvider _propertyProvider;
        private IBindableVisualElementsCreator _bindableElementsCreator;
        private Dictionary<string, HashSet<IBindableVisualElement>> _bindableVisualElements;

        public TBindingContext BindingContext => _bindingContext;

        public void Configure(TBindingContext bindingContext, IBindableVisualElementsCreator visualElementsCreator,
            IEnumerable<IValueConverter> valueConverters)
        {
            _bindingContext = bindingContext;
            _propertyProvider = new PropertyProvider<TBindingContext>(bindingContext, valueConverters);
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

        public IBindableElement RegisterBindableElement(IBindableUIElement bindableUiElement)
        {
            var bindableElement = _bindableElementsCreator.Create(bindableUiElement, _propertyProvider);
            if (bindableElement is not IBindableVisualElement bindableVisualElement)
            {
                return bindableElement;
            }

            foreach (var propertyName in bindableVisualElement.BindableProperties)
            {
                RegisterBindableElement(propertyName, bindableVisualElement);
            }

            bindableVisualElement.UpdateValues();
            return bindableElement;
        }

        private void RegisterBindableElement(string propertyName, IBindableVisualElement bindableVisualElement)
        {
            if (_bindableVisualElements.TryGetValue(propertyName, out var visualElements))
            {
                visualElements.Add(bindableVisualElement);
            }
            else
            {
                _bindableVisualElements.Add(propertyName,
                    new HashSet<IBindableVisualElement> { bindableVisualElement });
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