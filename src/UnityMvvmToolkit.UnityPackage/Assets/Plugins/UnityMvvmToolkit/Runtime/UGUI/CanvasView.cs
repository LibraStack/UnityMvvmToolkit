using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.UGUI
{
    public abstract class CanvasView<TBindingContext> : MonoBehaviour
        where TBindingContext : class, INotifyPropertyChanged
    {
        private GameObject _root;
        private View<TBindingContext> _view;
        private List<IDisposable> _disposables;

        public GameObject RootElement => _root;
        public TBindingContext BindingContext => _view.BindingContext;

        private void Awake()
        {
            _root = gameObject;
            _view = CreateView(GetBindingContext(), GetBindableElementsWrapper());
            _disposables = new List<IDisposable>();

            BindElements(_root); // TODO: Move to start?
        }

        private void OnEnable()
        {
            _view.EnableBinding();
        }

        private void OnDisable()
        {
            _view.DisableBinding();
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        protected virtual TBindingContext GetBindingContext()
        {
            // TODO: Change DataContext dynamically?

            if (typeof(TBindingContext).GetConstructor(Type.EmptyTypes) == null)
            {
                throw new InvalidOperationException(
                    $"Cannot create an instance of the type parameter {typeof(TBindingContext)} because it does not have a parameterless constructor.");
            }

            return Activator.CreateInstance<TBindingContext>();
        }

        protected virtual IBindableElementsWrapper GetBindableElementsWrapper()
        {
            return new BindableElementsWrapper();
        }

        protected virtual IConverter[] GetValueConverters()
        {
            return null;
        }

        protected virtual IObjectProvider GetObjectProvider(TBindingContext bindingContext, IConverter[] converters)
        {
            return new BindingContextObjectProvider<TBindingContext>(bindingContext, converters);
        }

        private View<TBindingContext> CreateView(TBindingContext bindingContext,
            IBindableElementsWrapper bindableElementsWrapper)
        {
            return new View<TBindingContext>()
                .Configure(bindingContext, GetObjectProvider(bindingContext, GetValueConverters()),
                    bindableElementsWrapper);
        }

        private void BindElements(GameObject rootElement)
        {
            foreach (var bindableUIElement in rootElement.GetComponentsInChildren<IBindableUIElement>(true))
            {
                var bindableElement = _view.RegisterBindableElement(bindableUIElement, true);
                if (bindableElement is IDisposable disposable)
                {
                    _disposables.Add(disposable);
                }
            }
        }
    }
}