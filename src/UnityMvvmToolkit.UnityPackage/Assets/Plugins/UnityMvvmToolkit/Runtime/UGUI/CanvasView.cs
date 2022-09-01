using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UGUI
{
    public abstract class CanvasView<TBindingContext> : MonoBehaviourView<TBindingContext>
        where TBindingContext : class, INotifyPropertyChanged
    {
        public GameObject RootElement { get; private set; }

        protected override void OnInit()
        {
            RootElement = gameObject;
        }

        protected override IBindableElementsFactory GetBindableElementsFactory()
        {
            return new BindableElementsFactory();
        }

        protected override IEnumerable<IBindableUIElement> GetBindableUIElements()
        {
            return RootElement.GetComponentsInChildren<IBindableUIElement>(true);
        }
    }
}