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

            TryInitialize(bindableElement);
            TryRegisterPropertyElement(bindableElement, updateElementValues);

            return bindableElement;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        private void TryInitialize(IBindableElement bindableElement)
        {
            var canInitialize = false;

            if (bindableElement is IInitializable { CanInitialize: true } initializable)
            {
                canInitialize = true;
                initializable.Initialize();
            }

            if (canInitialize && bindableElement is IDisposable disposable)
            {
                _disposables.Add(disposable);
            }
        }

        private void TryRegisterPropertyElement(IBindableElement bindableElement, bool updateElementValues)
        {
            if (bindableElement is not IBindablePropertyElement bindablePropertyElement)
            {
                return;
            }

            foreach (var propertyName in bindablePropertyElement.BindableProperties)
            {
                RegisterBindableElement(propertyName, bindablePropertyElement);
            }

            if (updateElementValues && bindablePropertyElement.BindableProperties.Count > 0)
            {
                bindablePropertyElement.UpdateValues();
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