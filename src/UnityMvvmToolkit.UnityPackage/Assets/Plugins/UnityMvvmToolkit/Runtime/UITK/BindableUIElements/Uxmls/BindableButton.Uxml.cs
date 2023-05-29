using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindableButton
    {
        public string Command { get; private set; }

        public new class UxmlFactory : UxmlFactory<BindableButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : BaseButton.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _commandAttribute = new()
                { name = "command", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                visualElement.As<BindableButton>().Command = _commandAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}