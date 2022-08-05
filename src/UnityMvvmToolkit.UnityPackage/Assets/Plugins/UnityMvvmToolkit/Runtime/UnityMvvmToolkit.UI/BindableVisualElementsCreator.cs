using System;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;
using UnityMvvmToolkit.UI.BindableVisualElements;

namespace UnityMvvmToolkit.UI
{
    public class BindableVisualElementsCreator : IBindableVisualElementsCreator
    {
        public virtual IBindableElement Create(IBindableUIElement bindableUiElement, IObjectProvider objectProvider)
        {
            return bindableUiElement switch
            {
                BindableLabel label => new BindableVisualLabel(label, objectProvider),
                BindableTextField textField => new BindableVisualTextField(textField, objectProvider),
                BindableButton button => new BindableVisualButton(button, objectProvider),

                _ => throw new NotImplementedException(
                    $"Bindable visual element for {bindableUiElement.GetType()} is not implemented.")
            };
        }
    }
}