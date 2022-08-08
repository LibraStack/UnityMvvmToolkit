using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Common
{
    public abstract class MonoBehaviourView<TBindingContext> : MonoBehaviour
        where TBindingContext : class, INotifyPropertyChanged
    {
        private View<TBindingContext> _view;

        public TBindingContext BindingContext => _view.BindingContext;

        private void Awake()
        {
            _view = CreateView(GetBindingContext(), GetBindableElementsWrapper());

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
        protected abstract IBindableElementsWrapper GetBindableElementsWrapper();
        protected abstract IEnumerable<IBindableUIElement> GetBindableUIElements();

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

        private void BindElements()
        {
            foreach (var bindableUIElement in GetBindableUIElements())
            {
                _view.RegisterBindableElement(bindableUIElement, true);
            }
        }
    }
}
