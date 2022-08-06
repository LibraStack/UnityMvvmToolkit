using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.UI
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class DocumentView<TBindingContext> : MonoBehaviour
        where TBindingContext : class, INotifyPropertyChanged
    {
        private UIDocument _uiDocument;
        private View<TBindingContext> _view;
        private List<IDisposable> _disposables;

        public TBindingContext BindingContext => _view.BindingContext;
        public VisualElement RootVisualElement => _uiDocument.rootVisualElement;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();

            _view = CreateView(GetBindingContext(), GetBindableElementsWrapper());
            _disposables = new List<IDisposable>();

            BindElements(_uiDocument.rootVisualElement); // TODO: Move to start?
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

        private void BindElements(VisualElement rootVisualElement)
        {
            rootVisualElement.Query<VisualElement>().ForEach(visualElement =>
            {
                if (visualElement is not IBindableUIElement bindableUIElement)
                {
                    return;
                }

                var bindableElement = _view.RegisterBindableElement(bindableUIElement, true);
                if (bindableElement is IDisposable disposable)
                {
                    _disposables.Add(disposable);
                }
            });
        }
    }
}