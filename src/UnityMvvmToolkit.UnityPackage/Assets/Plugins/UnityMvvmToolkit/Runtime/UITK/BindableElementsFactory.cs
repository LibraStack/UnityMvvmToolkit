using System;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.BindableUIElements;
using UnityMvvmToolkit.UITK.BindableUIElementWrappers;

namespace UnityMvvmToolkit.UITK
{
    public class BindableElementsFactory : IBindableElementsFactory
    {
        public virtual IBindableElement Create(IBindableUIElement bindableUiElement, IObjectProvider objectProvider)
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