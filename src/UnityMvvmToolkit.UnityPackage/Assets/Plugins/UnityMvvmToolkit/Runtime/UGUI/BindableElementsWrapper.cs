using System;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UGUI.BindableUGUIElements;
using UnityMvvmToolkit.UGUI.BindableUGUIElementWrappers;

namespace UnityMvvmToolkit.UGUI
{
    public class BindableElementsWrapper : IBindableElementsWrapper
    {
        public virtual IBindableElement Wrap(IBindableUIElement bindableUiElement, IObjectProvider objectProvider)
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