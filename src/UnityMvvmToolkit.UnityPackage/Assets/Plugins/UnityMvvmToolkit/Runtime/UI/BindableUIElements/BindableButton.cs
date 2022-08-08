using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UI.BindableUIElements
{
    public class BindableButton : Button, IBindableUIElement
    {
        public string Command { get; set; }

        public new class UxmlFactory : UxmlFactory<BindableButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : Button.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _commandAttribute = new()
                { name = "command", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((BindableButton) visualElement).Command = _commandAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}