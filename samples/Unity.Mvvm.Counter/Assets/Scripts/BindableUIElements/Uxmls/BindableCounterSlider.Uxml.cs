using UIElements;
using UnityEngine.UIElements;

namespace BindableUIElements
{
    public partial class BindableCounterSlider
    {
        public string IncrementCommand { get; set; }
        public string DecrementCommand { get; set; }

        public new class UxmlFactory : UxmlFactory<BindableCounterSlider, UxmlTraits>
        {
        }

        public new class UxmlTraits : CounterSlider.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _incrementCommandAttribute = new()
                { name = "increment-command", defaultValue = "" };

            private readonly UxmlStringAttributeDescription _decrementCommandAttribute = new()
                { name = "decrement-command", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                var bindableCounterSlider = (BindableCounterSlider) visualElement;
                bindableCounterSlider.IncrementCommand = _incrementCommandAttribute.GetValueFromBag(bag, context);
                bindableCounterSlider.DecrementCommand = _decrementCommandAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}