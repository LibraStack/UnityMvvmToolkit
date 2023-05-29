using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BaseButton
    {
        public new class UxmlTraits : Button.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription _enabledAttribute = new()
                { name = "enabled", defaultValue = true };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                visualElement.As<BaseButton>().Enabled = _enabledAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}