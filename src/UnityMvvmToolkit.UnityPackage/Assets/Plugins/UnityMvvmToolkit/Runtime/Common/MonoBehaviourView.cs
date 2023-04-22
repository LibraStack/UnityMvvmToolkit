using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityMvvmToolkit.Common.Interfaces;
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
            SetBindingContext(GetBindableElements(), GetBindingContext(), GetObjectProvider(), true);
        }

        private void OnDestroy()
        {
            ResetBindingContext(GetBindableElements(), _objectProvider, true);
        }

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            if (_bindingContext != null)
            {
                throw new InvalidOperationException(
                    $"{GetType().Name} - binding context was not reset. Reset the binding context first.");
            }

            _bindingContext = (TBindingContext) context;
            _objectProvider = objectProvider;

            SetBindingContext(GetBindableElements(), context, objectProvider, false);
        }

        public void ResetBindingContext(IObjectProvider objectProvider)
        {
            ResetBindingContext(GetBindableElements(), objectProvider, false);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetBindingContext(ReadOnlySpan<IBindableElement> bindableElements, IBindingContext context,
            IObjectProvider objectProvider, bool initialize)
        {
            // TODO: Self set;

            _bindingContext = (TBindingContext) context;
            _objectProvider = objectProvider;

            for (var i = 0; i < bindableElements.Length; i++)
            {
                var bindableElement = bindableElements[i];

                if (initialize && bindableElement is IInitializable initializable)
                {
                    initializable.Initialize();
                }

                bindableElement.SetBindingContext(context, objectProvider);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResetBindingContext(ReadOnlySpan<IBindableElement> bindableElements,
            IObjectProvider objectProvider, bool dispose)
        {
            // TODO: Self reset;

            for (var i = 0; i < bindableElements.Length; i++)
            {
                var bindableElement = bindableElements[i];

                if (objectProvider != null)
                {
                    bindableElement.ResetBindingContext(objectProvider);
                }

                if (dispose && bindableElement is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _bindingContext = null;
            _objectProvider = null;
        }
    }
}
