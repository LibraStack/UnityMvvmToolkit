using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace BindableUIElements
{
    public abstract partial class BindableBinaryStateButton
    {
        public new class UxmlTraits : BindableButton.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _stateAttribute = new()
                { name = "binding-state-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((BindableBinaryStateButton) visualElement).BindingStatePath =
                    _stateAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}