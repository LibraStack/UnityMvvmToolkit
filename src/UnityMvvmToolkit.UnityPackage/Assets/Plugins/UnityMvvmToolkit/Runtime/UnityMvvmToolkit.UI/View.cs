using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.Interfaces;

namespace UnityMvvmToolkit.UI
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class View<TBindingContext> : MonoBehaviour where TBindingContext : class, INotifyPropertyChanged
    {
        private UIDocument _uiDocument;
        private TBindingContext _bindingContext;

        private List<IDisposable> _disposables;
        private IBindableVisualElementsCreator _bindableElementsCreator;
        private Dictionary<string, HashSet<IBindableVisualElement>> _bindableVisualElements;

        public TBindingContext BindingContext => _bindingContext;
        public VisualElement RootVisualElement => _uiDocument.rootVisualElement;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            _bindingContext = GetBindingContext();

            _disposables = new List<IDisposable>();
            _bindableElementsCreator = GetBindableVisualElementsCreator();
            _bindableVisualElements = new Dictionary<string, HashSet<IBindableVisualElement>>();

            BindElements(_bindingContext, _uiDocument.rootVisualElement);
        }

        private void OnEnable()
        {
            _bindingContext.PropertyChanged += OnBindingContextPropertyChanged;
        }

        private void OnDisable()
        {
            _bindingContext.PropertyChanged -= OnBindingContextPropertyChanged;
        }

        private void OnDestroy()
        {
            // TODO: Unregister bindable elements (like TextField).

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

        protected virtual IBindableVisualElementsCreator GetBindableVisualElementsCreator()
        {
            return new BindableVisualElementsCreator();
        }

        private void BindElements(TBindingContext bindingContext, VisualElement rootVisualElement)
        {
            rootVisualElement.Query<VisualElement>().ForEach(visualElement =>
            {
                if (visualElement is not IBindableUIElement bindableUIElement)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(bindableUIElement.BindablePropertyName) == false)
                {
                    RegisterBindableElement(bindableUIElement, bindingContext);
                }
            });
        }

        private void RegisterBindableElement(IBindableUIElement bindableUIElement, TBindingContext bindingContext)
        {
            var propertyInfo = bindingContext.GetType().GetProperty(bindableUIElement.BindablePropertyName);
            if (propertyInfo == null)
            {
                throw new NullReferenceException(nameof(propertyInfo));
            }

            var bindableElement = _bindableElementsCreator.Create(bindingContext, bindableUIElement, propertyInfo);
            if (bindableElement is IBindableVisualElement bindableVisualElement)
            {
                RegisterBindableElement(bindableVisualElement, bindableUIElement.BindablePropertyName);
            }

            if (bindableElement is IDisposable disposable)
            {
                _disposables.Add(disposable);
            }
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
        
        protected virtual void OnBindingContextPropertyChanged(object sender, PropertyChangedEventArgs e)
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