using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindingContextProvider<TBindingContext>
    {
        public string BindingContextPath { get; protected set; }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingContextPath = new()
                { name = "binding-context-path", defaultValue = default };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                visualElement
                    .As<BindingContextProvider<TBindingContext>>()
                    .BindingContextPath = _bindingContextPath.GetValueFromBag(bag, context);
            }
        }
    }
}