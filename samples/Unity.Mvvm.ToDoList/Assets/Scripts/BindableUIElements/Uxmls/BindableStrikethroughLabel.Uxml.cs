using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace BindableUIElements
{
    public partial class BindableStrikethroughLabel
    {
        public string BindingIsDonePath { get; private set; }

        public new class UxmlFactory : UxmlFactory<BindableStrikethroughLabel, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableLabel.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingIsDoneAttribute = new()
                { name = "binding-is-done-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((BindableStrikethroughLabel) visualElement).BindingIsDonePath =
                    _bindingIsDoneAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}