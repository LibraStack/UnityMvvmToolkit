using System.Linq;
using UnityEngine;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UGUI
{
    public abstract class CanvasView<TBindingContext> : MonoBehaviourView<TBindingContext>
        where TBindingContext : class, IBindingContext
    {
        private IBindableElement[] _bindableElements;

        public GameObject RootElement { get; private set; }

        protected override void OnInit()
        {
            RootElement = gameObject;

            _bindableElements = RootElement
                .GetComponentsInChildren<IBindableElement>(true)
                .Where(element => ((MonoBehaviour) element).gameObject != RootElement)
                .ToArray();
        }

        protected override IBindableElement[] GetBindableElements()
        {
            return _bindableElements;
        }
    }
}