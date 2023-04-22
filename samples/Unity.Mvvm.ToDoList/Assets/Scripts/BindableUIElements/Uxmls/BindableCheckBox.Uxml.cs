using UIElements;
using UnityEngine.UIElements;

namespace BindableUIElements
{
    public partial class BindableCheckBox
    {
        public string BindingTextPath { get; private set; }
        public string BindingIsCheckedPath { get; private set; }

        public new class UxmlFactory : UxmlFactory<BindableCheckBox, UxmlTraits>
        {
        }

        public new class UxmlTraits : CheckBox.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingTextAttribute = new()
                { name = "binding-text-path", defaultValue = "" };

            private readonly UxmlStringAttributeDescription _bindingIsCheckedAttribute = new()
                { name = "binding-is-checked-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                var bindableCheckBox = (BindableCheckBox) visualElement;
                bindableCheckBox.BindingTextPath = _bindingTextAttribute.GetValueFromBag(bag, context);
                bindableCheckBox.BindingIsCheckedPath = _bindingIsCheckedAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}