using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class DocumentView<TBindingContext> : MonoBehaviourView<TBindingContext>
        where TBindingContext : class, IBindingContext
    {
        private UIDocument _uiDocument;
        private IBindableElement[] _bindableElements;

        public VisualElement RootVisualElement => _uiDocument == null ? null : _uiDocument.rootVisualElement;

        protected override void OnInit()
        {
            _uiDocument = GetComponent<UIDocument>();
            _bindableElements = RootVisualElement
                .Query<VisualElement>()
                .Where(element => element is IBindableElement)
                .Build()
                .Cast<IBindableElement>()
                .ToArray();
        }

        protected override IBindableElement[] GetBindableElements()
        {
            return _bindableElements;
        }
    }
}