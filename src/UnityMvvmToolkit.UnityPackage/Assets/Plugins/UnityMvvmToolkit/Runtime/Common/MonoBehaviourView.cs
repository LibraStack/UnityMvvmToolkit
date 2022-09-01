using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Common
{
    [DefaultExecutionOrder(1)]
    public abstract class MonoBehaviourView<TBindingContext> : MonoBehaviour
        where TBindingContext : class, INotifyPropertyChanged
    {
        private View<TBindingContext> _view;

        public TBindingContext BindingContext => _view.BindingContext;

        private void Awake()
        {
            _view = CreateView(GetBindingContext(), GetBindableElementsFactory());

            OnInit();
            BindElements();
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
            _view.Dispose();
        }

        protected abstract void OnInit();
        protected abstract IBindableElementsFactory GetBindableElementsFactory();
        protected abstract IEnumerable<IBindableUIElement> GetBindableUIElements();

        protected virtual TBindingContext GetBindingContext()
        {
            if (typeof(TBindingContext).GetConstructor(Type.EmptyTypes) == null)
            {
                throw new InvalidOperationException(
                    $"Cannot create an instance of the type parameter {typeof(TBindingContext)} because it does not have a parameterless constructor.");
            }

            return Activator.CreateInstance<TBindingContext>();
        }

        protected virtual IValueConverter[] GetValueConverters()
        {
            return Array.Empty<IValueConverter>();
        }

        protected virtual IObjectProvider GetObjectProvider(TBindingContext bindingContext, IValueConverter[] converters)
        {
            return new BindingContextObjectProvider<TBindingContext>(bindingContext, converters);
        }

        private View<TBindingContext> CreateView(TBindingContext bindingContext,
            IBindableElementsFactory bindableElementsFactory)
        {
            return new View<TBindingContext>()
                .Configure(bindingContext, GetObjectProvider(bindingContext, GetValueConverters()),
                    bindableElementsFactory);
        }

        private void BindElements()
        {
            foreach (var bindableUIElement in GetBindableUIElements())
            {
                _view.RegisterBindableElement(bindableUIElement, true);
            }
        }
    }
}
