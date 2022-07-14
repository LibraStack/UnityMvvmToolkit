using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Attributes;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.UI.BindableVisualElements
{
    public class BindableTextField : TextField, IBindableVisualElement
    {
        [BindTo(nameof(value))]
        public string BindingValuePath { get; set; }

        public new class UxmlFactory : UxmlFactory<BindableTextField, UxmlTraits>
        {
        }

        public new class UxmlTraits : TextField.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingValueAttribute = new()
                { name = "binding-value-path", defaultValue = "binding-property-name" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((BindableTextField) visualElement).BindingValuePath =
                    _bindingValueAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}