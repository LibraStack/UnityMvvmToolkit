using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Extensions;

// ReSharper disable SuspiciousTypeConversion.Global

namespace UnityMvvmToolkit.Common
{
    [DefaultExecutionOrder(1)]
    public abstract class MonoBehaviourView<TBindingContext> : MonoBehaviour, IBindableElement
        where TBindingContext : class, IBindingContext
    {
        private bool _isInitialized;

        private TBindingContext _bindingContext;
        private IObjectProvider _objectProvider;

        private TBindingContext _createdBindingContext;

        protected virtual bool InitOnAwake => true;
        public TBindingContext BindingContext => _bindingContext;

        private void Awake()
        {
            if (InitOnAwake)
            {
                Init();
            }
        }

        private void OnDestroy()
        {
            ResetBindingContext();
            OnDispose();
        }

        protected void Init()
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{GetType().Name} already initialized.");
            }

            OnInit();
            SetBindingContext();

            _isInitialized = true;
        }

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            if (_bindingContext is not null)
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
        protected abstract void OnDispose();
        protected abstract IReadOnlyList<IBindableElement> GetBindableElements();

        protected virtual TBindingContext GetBindingContext()
        {
            if (typeof(TBindingContext).GetConstructor(Type.EmptyTypes) is null)
            {
                throw new InvalidOperationException(
                    $"Cannot create an instance of the type parameter {typeof(TBindingContext)} because it does not have a parameterless constructor.");
            }

            _createdBindingContext = Activator.CreateInstance<TBindingContext>();

            return _createdBindingContext;
        }

        protected virtual IObjectProvider GetObjectProvider()
        {
            return new BindingContextObjectProvider(GetValueConverters(), GetCollectionItemTemplates());
        }

        protected virtual IValueConverter[] GetValueConverters()
        {
            return Array.Empty<IValueConverter>();
        }

        protected virtual IReadOnlyDictionary<Type, object> GetCollectionItemTemplates()
        {
            return ImmutableDictionary.Empty<Type, object>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetBindingContext()
        {
            var bindingContext = GetBindingContext();

            if (bindingContext == _createdBindingContext &&
                bindingContext is IInitializable initializable)
            {
                initializable.Initialize();
            }

            SetBindingContext(GetBindableElements(), bindingContext, GetObjectProvider(), true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResetBindingContext()
        {
            ResetBindingContext(GetBindableElements(), _objectProvider, true);

            if (_createdBindingContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetBindingContext(IReadOnlyList<IBindableElement> bindableElements, IBindingContext context,
            IObjectProvider objectProvider, bool initialize)
        {
            _bindingContext = (TBindingContext) context;
            _objectProvider = objectProvider;

            for (var i = 0; i < bindableElements.Count; i++)
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
        private void ResetBindingContext(IReadOnlyList<IBindableElement> bindableElements,
            IObjectProvider objectProvider, bool dispose)
        {
            for (var i = 0; i < bindableElements.Count; i++)
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
