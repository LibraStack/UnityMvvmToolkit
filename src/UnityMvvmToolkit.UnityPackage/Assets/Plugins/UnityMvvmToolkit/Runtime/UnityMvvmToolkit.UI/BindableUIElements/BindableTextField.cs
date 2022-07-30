using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.UI.BindableUIElements
{
    public class BindableTextField : TextField, IBindableUIElement
    {
        public string BindingValuePath { get; set; }

        public string BindablePropertyName => BindingValuePath;
        
        public new class UxmlFactory : UxmlFactory<BindableTextField, UxmlTraits>
        {
        }

        public new class UxmlTraits : TextField.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingValueAttribute = new()
                { name = "binding-value-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((BindableTextField) visualElement).BindingValuePath =
                    _bindingValueAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}