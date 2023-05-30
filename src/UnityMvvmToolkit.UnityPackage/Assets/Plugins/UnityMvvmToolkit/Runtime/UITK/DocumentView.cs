using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class DocumentView<TBindingContext> : MonoBehaviourView<TBindingContext>
        where TBindingContext : class, IBindingContext
    {
        private UIDocument _uiDocument;
        private List<IBindableElement> _bindableElements;

        public VisualElement RootVisualElement => _uiDocument == null ? null : _uiDocument.rootVisualElement;

        protected override void OnInit()
        {
            _uiDocument = GetComponent<UIDocument>();
            _bindableElements = RootVisualElement.GetBindableChilds();
        }

        protected override IReadOnlyList<IBindableElement> GetBindableElements()
        {
            return _bindableElements;
        }

        protected override void OnDispose()
        {
            _bindableElements.Clear();
        }
    }
}