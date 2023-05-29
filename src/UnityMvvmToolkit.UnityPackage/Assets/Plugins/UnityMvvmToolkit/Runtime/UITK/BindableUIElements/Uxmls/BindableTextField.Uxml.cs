using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindableTextField
    {
        public string BindingValuePath { get; private set; }

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

                visualElement
                    .As<BindableTextField>()
                    .BindingValuePath = _bindingValueAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}