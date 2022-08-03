using UIElements;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableUIElements
{
    public class BindableCounterSlider : CounterSlider, IBindableUIElement
    {
        public string IncreaseCommand { get; set; }
        public string DecreaseCommand { get; set; }

        public new class UxmlFactory : UxmlFactory<BindableCounterSlider, UxmlTraits>
        {
        }

        public new class UxmlTraits : CounterSlider.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _increaseCommandAttribute = new()
                { name = "increase-command", defaultValue = "" };

            private readonly UxmlStringAttributeDescription _decreaseCommandAttribute = new()
                { name = "decrease-command", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                var bindableCounterSlider = (BindableCounterSlider) visualElement;
                bindableCounterSlider.IncreaseCommand = _increaseCommandAttribute.GetValueFromBag(bag, context);
                bindableCounterSlider.DecreaseCommand = _decreaseCommandAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}