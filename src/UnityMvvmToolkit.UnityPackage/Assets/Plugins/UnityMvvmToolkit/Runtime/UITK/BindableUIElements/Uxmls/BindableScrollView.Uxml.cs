using UnityEngine.UIElements;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public partial class BindableScrollView<TItemBindingContext>
    {
        public string BindingItemsSourcePath { get; private set; }

        public new class UxmlTraits : ScrollView.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingItemsSourceAttribute = new()
                { name = "binding-items-source-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                var bindableScrollView = (BindableScrollView<TItemBindingContext>) visualElement;
                bindableScrollView.BindingItemsSourcePath = _bindingItemsSourceAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}