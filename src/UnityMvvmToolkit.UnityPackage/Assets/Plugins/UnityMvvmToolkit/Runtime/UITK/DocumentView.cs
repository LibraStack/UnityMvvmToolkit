using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class DocumentView<TBindingContext> : MonoBehaviourView<TBindingContext>
        where TBindingContext : class, INotifyPropertyChanged
    {
        private UIDocument _uiDocument;

        public VisualElement RootVisualElement => _uiDocument == null ? null : _uiDocument.rootVisualElement;

        protected override void OnInit()
        {
            _uiDocument = GetComponent<UIDocument>();
        }

        protected override IBindableElementsWrapper GetBindableElementsWrapper()
        {
            return new BindableElementsWrapper();
        }

        protected override IEnumerable<IBindableUIElement> GetBindableUIElements()
        {
            return RootVisualElement
                .Query<VisualElement>()
                .Where(element => element is IBindableUIElement)
                .Build()
                .Cast<IBindableUIElement>();
        }
    }
}