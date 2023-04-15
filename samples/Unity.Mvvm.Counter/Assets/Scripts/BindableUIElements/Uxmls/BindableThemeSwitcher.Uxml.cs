using UIElements;
using UnityEngine.UIElements;

namespace BindableUIElements
{
    public partial class BindableThemeSwitcher
    {
        public string BindingValuePath { get; private set; }

        public new class UxmlFactory : UxmlFactory<BindableThemeSwitcher, UxmlTraits>
        {
        }

        public new class UxmlTraits : ThemeSwitcher.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingValueAttribute = new()
                { name = "binding-value-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((BindableThemeSwitcher) visualElement).BindingValuePath =
                    _bindingValueAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}