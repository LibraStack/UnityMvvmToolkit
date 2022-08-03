using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.UI.BindableUIElements
{
    public class BindableButton : Button, IBindableUIElement
    {
        public string Command { get; set; }
        public string CommandParameter { get; set; }

        public new class UxmlFactory : UxmlFactory<BindableButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : Button.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _commandAttribute = new()
                { name = "command", defaultValue = "" };
            
            private readonly UxmlStringAttributeDescription _commandParameterAttribute = new()
                { name = "command-parameter", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                var bindableButton = (BindableButton) visualElement;
                bindableButton.Command = _commandAttribute.GetValueFromBag(bag, context);
                bindableButton.CommandParameter = _commandParameterAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}