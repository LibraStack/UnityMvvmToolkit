using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public class BindableButton : Button, IBindableUIElement
    {
        public bool Enabled
        {
            get => enabledSelf;
            set => SetEnabled(value);
        }

        public string Command { get; set; }

        public new class UxmlFactory : UxmlFactory<BindableButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : Button.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription _enabledAttribute = new()
                { name = "enabled", defaultValue = true };

            private readonly UxmlStringAttributeDescription _commandAttribute = new()
                { name = "command", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                var bindableButton = (BindableButton) visualElement;
                bindableButton.Enabled = _enabledAttribute.GetValueFromBag(bag, context);
                bindableButton.Command = _commandAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}