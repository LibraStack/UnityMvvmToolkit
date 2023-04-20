using UnityEngine.UIElements;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public class ButtonUITK : Button
    {
        public bool Enabled
        {
            get => enabledSelf;
            set => SetEnabled(value);
        }

        public new class UxmlFactory : UxmlFactory<ButtonUITK, UxmlTraits>
        {
        }

        public new class UxmlTraits : Button.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription _enabledAttribute = new()
                { name = "enabled", defaultValue = true };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                var bindableButton = (ButtonUITK) visualElement;
                bindableButton.Enabled = _enabledAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}