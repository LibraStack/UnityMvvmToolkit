using System;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;
using UnityMvvmToolkit.UI.BindableUIElementWrappers;

namespace UnityMvvmToolkit.UI
{
    public class BindableElementsWrapper : IBindableElementsWrapper
    {
        public virtual IBindableElement Wrap(IBindableUIElement bindableUiElement, IObjectProvider objectProvider)
        {
            return bindableUiElement switch
            {
                BindableLabel label => new BindableLabelWrapper(label, objectProvider),
                BindableTextField textField => new BindableTextFieldWrapper(textField, objectProvider),
                BindableButton button => new BindableButtonWrapper(button, objectProvider),

                _ => throw new NotImplementedException(
                    $"Bindable visual element for {bindableUiElement.GetType()} is not implemented.")
            };
        }
    }
}