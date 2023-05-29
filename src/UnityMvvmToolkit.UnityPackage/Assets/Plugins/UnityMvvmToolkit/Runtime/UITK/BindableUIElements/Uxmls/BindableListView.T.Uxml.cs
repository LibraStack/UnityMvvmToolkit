using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindableListView<TItemBindingContext>
    {
        public string BindingItemsSourcePath { get; private set; }

        public new class UxmlTraits : ListView.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingItemsSourceAttribute = new()
                { name = "binding-items-source-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                visualElement
                    .As<BindableListView<TItemBindingContext>>()
                    .BindingItemsSourcePath = _bindingItemsSourceAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}