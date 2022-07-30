using UIElements;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableUIElements
{
    public class BindableCounterSlider : CounterSlider, IBindableUIElement
    {
        public string Command { get; set; }
        public string IncreaseCommandParameter { get; set; }
        public string DecreaseCommandParameter { get; set; }

        public string BindablePropertyName => Command;

        public new class UxmlFactory : UxmlFactory<BindableCounterSlider, UxmlTraits>
        {
        }

        public new class UxmlTraits : CounterSlider.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _commandAttribute = new()
                { name = "command", defaultValue = "" };

            private readonly UxmlStringAttributeDescription _increaseCommandParameterAttribute = new()
                { name = "increase-command-parameter", defaultValue = "" };
            
            private readonly UxmlStringAttributeDescription _decreaseCommandParameterAttribute = new()
                { name = "decrease-command-parameter", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                var bindableCounterSlider = (BindableCounterSlider) visualElement;
                bindableCounterSlider.Command = _commandAttribute.GetValueFromBag(bag, context);
                bindableCounterSlider.IncreaseCommandParameter =
                    _increaseCommandParameterAttribute.GetValueFromBag(bag, context);
                bindableCounterSlider.DecreaseCommandParameter =
                    _decreaseCommandParameterAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}