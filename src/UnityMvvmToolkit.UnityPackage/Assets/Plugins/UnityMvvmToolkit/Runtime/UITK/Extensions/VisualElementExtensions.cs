using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.Extensions
{
    public static partial class VisualElementExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VisualElement InstantiateBindableElement(this VisualTreeAsset visualTreeAsset)
        {
            var visualElement = visualTreeAsset.Instantiate();
            visualElement.userData = new BindableElementData(visualElement.GetBindableChilds());

            return visualElement;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<IBindableElement> GetBindableChilds(this VisualElement visualElement)
        {
            var bindableChilds = new List<IBindableElement>();

            var itemChildCount = visualElement.childCount;

            for (var i = 0; i < itemChildCount; i++)
            {
                GetBindableElements(visualElement[i], bindableChilds);
            }

            return bindableChilds;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VisualElement InitializeBindableElement(this VisualElement visualElement)
        {
            if (visualElement is IInitializable initializable)
            {
                initializable.Initialize();
            }

            var bindableElements = ((BindableElementData) visualElement.userData).BindableElements;

            for (var i = 0; i < bindableElements.Count; i++)
            {
                if (bindableElements[i] is IInitializable initializableChild)
                {
                    initializableChild.Initialize();
                }
            }

            return visualElement;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TBindingContext GetBindingContext<TBindingContext>(this VisualElement visualElement)
            where TBindingContext : IBindingContext
        {
            return (TBindingContext) ((BindableElementData) visualElement.userData).BindingContext;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetChildsBindingContext(this VisualElement visualElement, IBindingContext bindingContext,
            IObjectProvider objectProvider)
        {
            var bindableElementData = (BindableElementData) visualElement.userData;
            bindableElementData.BindingContext = bindingContext;

            var bindableElements = bindableElementData.BindableElements;

            for (var i = 0; i < bindableElements.Count; i++)
            {
                bindableElements[i].SetBindingContext(bindingContext, objectProvider);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetChildsBindingContext(this VisualElement visualElement, IObjectProvider objectProvider)
        {
            var bindableElementData = (BindableElementData) visualElement.userData;
            bindableElementData.BindingContext = default;

            var bindableElements = bindableElementData.BindableElements;

            for (var i = 0; i < bindableElements.Count; i++)
            {
                bindableElements[i].ResetBindingContext(objectProvider);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeBindableElement(this VisualElement visualElement,
            [CanBeNull] IObjectProvider objectProvider = null)
        {
            var bindableElementData = (BindableElementData) visualElement.userData;
            var bindableElements = bindableElementData.BindableElements;

            for (var i = 0; i < bindableElements.Count; i++)
            {
                var bindableElement = bindableElements[i];

                if (objectProvider is not null)
                {
                    bindableElement.ResetBindingContext(objectProvider);
                }

                if (bindableElement is IDisposable disposableChild)
                {
                    disposableChild.Dispose();
                }
            }

            if (visualElement is IDisposable disposable)
            {
                disposable.Dispose();
            }

            bindableElementData.Dispose();
            visualElement.userData = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetBindableElements(VisualElement visualElement, ICollection<IBindableElement> elements)
        {
            if (visualElement is IBindableElement bindableElement)
            {
                elements.Add(bindableElement);
            }

            if (visualElement is IBindableCollection or IBindingContextProvider { IsValid: true })
            {
                return;
            }

            if (visualElement is TemplateContainer)
            {
                var hierarchyChildCount = visualElement.hierarchy.childCount;

                for (var i = 0; i < hierarchyChildCount; i++)
                {
                    GetBindableElements(visualElement.hierarchy[i], elements);
                }
            }
            else
            {
                var childCount = visualElement.childCount;

                for (var i = 0; i < childCount; i++)
                {
                    GetBindableElements(visualElement[i], elements);
                }
            }
        }
    }
}