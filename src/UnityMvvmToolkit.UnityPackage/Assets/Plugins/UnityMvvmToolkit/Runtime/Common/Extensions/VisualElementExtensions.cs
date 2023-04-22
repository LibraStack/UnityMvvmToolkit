using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Common.Extensions
{
    public static class VisualElementExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VisualElement InstantiateBindableElement(this VisualTreeAsset visualTreeAsset)
        {
            var visualElement = visualTreeAsset.Instantiate();
            visualElement.userData = GetBindableElements(visualElement);

            return visualElement;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IBindableElement[] GetBindableElements(this VisualElement visualElement)
        {
            return visualElement
                .Query<VisualElement>()
                .Where(element => element is IBindableElement)
                .Build()
                .Cast<IBindableElement>()
                .ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBindingContext(this VisualElement visualElement, IBindingContext context,
            IObjectProvider objectProvider, bool initialize = false)
        {
            // if (visualElement is IBindableElement bindableElement)
            // {
            //     SetBindingContext(bindableElement, context, objectProvider, initialize);
            // }

            var bindableElementsSpan = ((IBindableElement[]) visualElement.userData).AsSpan();

            for (var i = 0; i < bindableElementsSpan.Length; i++)
            {
                SetBindingContext(bindableElementsSpan[i], context, objectProvider, initialize);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetBindingContext(this VisualElement visualElement, IObjectProvider objectProvider,
            bool dispose = false)
        {
            // if (visualElement is IBindableElement bindableElement)
            // {
            //     ResetBindingContext(bindableElement, objectProvider, dispose);
            // }

            var bindableElementsSpan = ((IBindableElement[]) visualElement.userData).AsSpan();

            for (var i = 0; i < bindableElementsSpan.Length; i++)
            {
                ResetBindingContext(bindableElementsSpan[i], objectProvider, dispose);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetBindingContext(IBindableElement bindableElement, IBindingContext context,
            IObjectProvider objectProvider, bool initialize)
        {
            if (initialize && bindableElement is IInitializable initializable)
            {
                initializable.Initialize();
            }

            bindableElement.ResetBindingContext(objectProvider);
            bindableElement.SetBindingContext(context, objectProvider);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ResetBindingContext(IBindableElement bindableElement, IObjectProvider objectProvider,
            bool disposeElements)
        {
            bindableElement.ResetBindingContext(objectProvider);

            if (disposeElements && bindableElement is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}