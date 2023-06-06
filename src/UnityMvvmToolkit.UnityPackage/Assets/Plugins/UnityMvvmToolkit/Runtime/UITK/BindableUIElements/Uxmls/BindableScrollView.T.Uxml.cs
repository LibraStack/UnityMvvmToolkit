using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindableScrollView<TItemBindingContext>
    {
        public string BindingItemsSourcePath { get; private set; }

#if UNITY_2023_2_OR_NEWER
        [System.Serializable]
        public new class UxmlSerializedData : ScrollView.UxmlSerializedData
        {
            // ReSharper disable once InconsistentNaming
            #pragma warning disable 649
            [UnityEngine.SerializeField] private string BindingItemsSourcePath;
            #pragma warning restore 649

            public override void Deserialize(object visualElement)
            {
                base.Deserialize(visualElement);

                visualElement
                    .As<BindableScrollView<TItemBindingContext>>()
                    .BindingItemsSourcePath = BindingItemsSourcePath;
            }
        }
#else
        public new class UxmlTraits : ScrollView.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingItemsSourceAttribute = new()
                { name = "binding-items-source-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                visualElement
                    .As<BindableScrollView<TItemBindingContext>>()
                    .BindingItemsSourcePath = _bindingItemsSourceAttribute.GetValueFromBag(bag, context);
            }
        }
#endif
    }
}