using System;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;
using UnityMvvmToolkit.UI.BindableVisualElements;

namespace UnityMvvmToolkit.UI
{
    public class BindableVisualElementsCreator : IBindableVisualElementsCreator
    {
        public virtual IBindableElement Create(IBindableUIElement bindableUiElement, IPropertyProvider propertyProvider)
        {
            return bindableUiElement switch
            {
                BindableLabel label => new BindableVisualLabel(label, propertyProvider),
                BindableTextField textField => new BindableVisualTextField(textField, propertyProvider),
                BindableButton button => new BindableVisualButton(button, propertyProvider),

                _ => throw new NotImplementedException(
                    $"Bindable visual element for {bindableUiElement.GetType()} is not implemented.")
            };
        }
    }
}