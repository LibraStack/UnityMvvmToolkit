using UIElements;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableUIElements
{
    public class BindableThemeSwitcher : ThemeSwitcher, IBindableUIElement
    {
        public string BindingValuePath { get; set; }
        
        public string BindablePropertyName => BindingValuePath;
        
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