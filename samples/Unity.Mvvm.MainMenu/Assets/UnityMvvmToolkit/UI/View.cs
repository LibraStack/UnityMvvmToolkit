using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.UI
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class View<TBindingContext> : MonoBehaviour where TBindingContext : class, INotifyPropertyChanged, new()
    {
        private UIDocument _uiDocument;
        private TBindingContext _bindingContext;
        private List<IDisposable> _disposables;
        private Dictionary<string, HashSet<IVisualElementBindings>> _visualElementsBindings;

        protected TBindingContext BindingContext => _bindingContext;
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            _bindingContext = GetBindingContext();
            
            _disposables = new List<IDisposable>();
            _visualElementsBindings = new Dictionary<string, HashSet<IVisualElementBindings>>();
            
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
            return new TBindingContext(); // TODO: Change DataContext dynamically?
        }

        protected virtual IVisualElementBindings GetVisualElementBindings(TBindingContext bindingContext,
            IBindableVisualElement bindableElement)
        {
            throw new NotImplementedException();
        }
        
        protected virtual void BindElements(TBindingContext bindingContext, VisualElement rootVisualElement)
        {
            // var bindingContextProperties =
            //     typeof(TBindingContext).GetProperties(BindingFlags.Public | BindingFlags.Instance |
            //                                           BindingFlags.DeclaredOnly);
            
            rootVisualElement.Query<VisualElement>().ForEach(visualElement =>
            {
                if (visualElement is IBindableVisualElement bindableElement)
                {
                    RegisterBindableElement(bindableElement);
                }
            });
        }

        private void RegisterBindableElement(IBindableVisualElement bindableElement)
        {
            var bindingProperties = bindableElement.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var bindingPropertyInfo in bindingProperties)
            {
                var sourcePropertyName = bindingPropertyInfo.GetValue(bindableElement).ToString();
                if (string.IsNullOrWhiteSpace(sourcePropertyName))
                {
                    continue;
                }

                var sourcePropertyInfo = typeof(TBindingContext).GetProperty(sourcePropertyName); // TODO: Cache properties to dictionary.
                if (sourcePropertyInfo == null)
                {
                    throw new NullReferenceException(nameof(sourcePropertyInfo));
                }

                var visualElementBindings = GetVisualElementBindings(_bindingContext, bindableElement);
                if (visualElementBindings == null)
                {
                    return;
                    throw new NullReferenceException(nameof(visualElementBindings));
                }

                if (visualElementBindings is IDisposable disposable)
                {
                    _disposables.Add(disposable);
                }

                if (_visualElementsBindings.TryGetValue(sourcePropertyName, out var visualElements) == false)
                {
                    visualElements = new HashSet<IVisualElementBindings>();
                    _visualElementsBindings.Add(sourcePropertyName, visualElements);
                }

                visualElementBindings.UpdateValues();
                visualElements.Add(visualElementBindings);
            }
        }
        protected virtual void OnBindingContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_visualElementsBindings.TryGetValue(e.PropertyName, out var visualElements))
            {
                foreach (var visualElement in visualElements)
                {
                    visualElement.UpdateValues();
                }
            }
        }
    }
}