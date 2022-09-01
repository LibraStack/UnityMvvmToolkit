using System;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UGUI.BindableUGUIElements;
using UnityMvvmToolkit.UGUI.BindableUGUIElementWrappers;

namespace UnityMvvmToolkit.UGUI
{
    public class BindableElementsFactory : IBindableElementsFactory
    {
        public virtual IBindableElement Create(IBindableUIElement bindableUiElement, IObjectProvider objectProvider)
        {
            return bindableUiElement switch
            {
                BindableLabel label => new BindableLabelWrapper(label, objectProvider),
                BindableInputField inputField => new BindableInputFieldWrapper(inputField, objectProvider),
                BindableButton button => new BindableButtonWrapper(button, objectProvider),

                _ => throw new NotImplementedException(
                    $"Bindable visual element for {bindableUiElement.GetType()} is not implemented.")
            };
        }
    }
}