using UnityEngine.UIElements;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public partial class BindableButton
    {
        public string Command { get; private set; }

        public new class UxmlFactory : UxmlFactory<BindableButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : ButtonUITK.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _commandAttribute = new()
                { name = "command", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                var bindableButton = (BindableButton) visualElement;
                bindableButton.Command = _commandAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}