using System;
using UnityEngine;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Common
{
    [DefaultExecutionOrder(1)]
    public abstract class MonoBehaviourView<TBindingContext> : MonoBehaviour, IBindableElement
        where TBindingContext : class, IBindingContext
    {
        private TBindingContext _bindingContext;
        private IObjectProvider _objectProvider;

        public TBindingContext BindingContext => _bindingContext;

        private void Awake()
        {
            OnInit();
            SetBindingContext(GetBindingContext(), GetObjectProvider());
        }

        private void OnDestroy()
        {
            if (_objectProvider != null)
            {
                ResetBindingContext(_objectProvider);
            }
        }

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _bindingContext = (TBindingContext) context;
            _objectProvider = objectProvider;

            var bindableElements = GetBindableElements().AsSpan();
            for (var i = 0; i < bindableElements.Length; i++)
            {
                bindableElements[i].SetBindingContext(context, objectProvider);
            }
        }

        public void ResetBindingContext(IObjectProvider objectProvider)
        {
            var bindableElements = GetBindableElements().AsSpan();
            for (var i = 0; i < bindableElements.Length; i++)
            {
                bindableElements[i].ResetBindingContext(objectProvider);
            }

            _bindingContext = null;
            _objectProvider = null;
        }

        protected abstract void OnInit();
        protected abstract IBindableElement[] GetBindableElements();

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

        protected virtual IObjectProvider GetObjectProvider()
        {
            return new BindingContextObjectProvider(GetValueConverters());
        }
    }
}
