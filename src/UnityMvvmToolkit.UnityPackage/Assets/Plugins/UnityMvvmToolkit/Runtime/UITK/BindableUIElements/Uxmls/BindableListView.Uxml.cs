using UnityEngine.UIElements;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public partial class BindableListView<TItemBindingContext>
    {
        public string BindingItemsSourcePath { get; private set; }
        public string BindingItemTemplatePath { get; private set; }

        public new class UxmlTraits : ListView.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingItemsSourceAttribute = new()
                { name = "binding-items-source-path", defaultValue = "" };

            private readonly UxmlStringAttributeDescription _bindingItemTemplateAttribute = new()
                { name = "binding-item-template-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                var bindableListView = (BindableListView<TItemBindingContext>) visualElement;
                bindableListView.BindingItemsSourcePath = _bindingItemsSourceAttribute.GetValueFromBag(bag, context);
                bindableListView.BindingItemTemplatePath = _bindingItemTemplateAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}