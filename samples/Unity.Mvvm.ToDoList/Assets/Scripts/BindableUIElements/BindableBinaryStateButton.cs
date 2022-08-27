using Interfaces;
using UnityEngine.UIElements;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace BindableUIElements
{
    public abstract class BindableBinaryStateButton : BindableButton, IBindableBinaryStateElement
    {
        public string BindingStatePath { get; set; }

        public abstract void Activate();
        public abstract void Deactivate();

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