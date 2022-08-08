using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core
{
    public class View<TBindingContext> : IDisposable where TBindingContext : class, INotifyPropertyChanged
    {
        private TBindingContext _bindingContext;
        private IObjectProvider _objectProvider;
        private IBindableElementsWrapper _bindableElementsWrapper;
        
        private List<IDisposable> _disposables;
        private Dictionary<string, HashSet<IBindablePropertyElement>> _bindablePropertyElements;

        public TBindingContext BindingContext => _bindingContext;

        public View<TBindingContext> Configure(TBindingContext bindingContext, IObjectProvider objectProvider,
            IBindableElementsWrapper elementsWrapper)
        {
            _bindingContext = bindingContext;
            _objectProvider = objectProvider;
            _bindableElementsWrapper = elementsWrapper;

            _disposables = new List<IDisposable>();
            _bindablePropertyElements = new Dictionary<string, HashSet<IBindablePropertyElement>>();

            return this;
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
            if (bindableElement is IDisposable disposable)
            {
                _disposables.Add(disposable);
            }

            if (bindableElement is not IBindablePropertyElement bindablePropertyElement)
            {
                return bindableElement;
            }

            foreach (var propertyName in bindablePropertyElement.BindableProperties)
            {
                RegisterBindableElement(propertyName, bindablePropertyElement);
            }

            if (updateElementValues)
            {
                bindablePropertyElement.UpdateValues();
            }

            return bindableElement;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        private void RegisterBindableElement(string propertyName, IBindablePropertyElement bindablePropertyElement)
        {
            if (_bindablePropertyElements.TryGetValue(propertyName, out var propertyElements))
            {
                propertyElements.Add(bindablePropertyElement);
            }
            else
            {
                _bindablePropertyElements.Add(propertyName,
                    new HashSet<IBindablePropertyElement> { bindablePropertyElement });
            }
        }

        private void OnBindingContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_bindablePropertyElements.TryGetValue(e.PropertyName, out var propertyElements))
            {
                foreach (var propertyElement in propertyElements)
                {
                    propertyElement.UpdateValues();
                }
            }
        }
    }
}