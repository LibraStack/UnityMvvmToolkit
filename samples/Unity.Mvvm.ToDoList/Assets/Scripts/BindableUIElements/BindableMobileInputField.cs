using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace BindableUIElements
{
    public class BindableMobileInputField : BindableTextField
    {
        public bool HideMobileInput
        {
            get => TouchScreenKeyboard.hideInput;
            set => TouchScreenKeyboard.hideInput = value;
        }

        public new class UxmlFactory : UxmlFactory<BindableMobileInputField, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableTextField.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription _hideMobileInputAttribute = new()
                { name = "hide-mobile-input", defaultValue = false };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((BindableMobileInputField) visualElement).HideMobileInput =
                    _hideMobileInputAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}