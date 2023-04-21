using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
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
        public static void SetBindingContext(this VisualElement visualElement, IBindingContext context,
            IObjectProvider objectProvider)
        {
            var bindableElementsSpan = ((IBindableElement[]) visualElement.userData).AsSpan();

            for (var i = 0; i < bindableElementsSpan.Length; i++)
            {
                bindableElementsSpan[i].ResetBindingContext(objectProvider);
                bindableElementsSpan[i].SetBindingContext(context, objectProvider);
            }
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
    }
}