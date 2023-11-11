using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindableListView<TItemBindingContext, TCollection>
    {
        public string BindingItemsSourcePath { get; private set; }
        public VisualTreeAsset ItemTemplate { get; private set; }

#if UNITY_2023_2_OR_NEWER
        [System.Serializable]
        public new class UxmlSerializedData : ListView.UxmlSerializedData
        {
            #pragma warning disable 649
            // ReSharper disable once InconsistentNaming
            [UnityEngine.SerializeField] private string BindingItemsSourcePath;
            // ReSharper disable once InconsistentNaming
            [UnityEngine.SerializeField] private VisualTreeAsset ItemTemplate;
            #pragma warning restore 649

            public override void Deserialize(object visualElement)
            {
                base.Deserialize(visualElement);

                var bindableListView = visualElement.As<BindableListView<TItemBindingContext, TCollection>>();
                bindableListView.BindingItemsSourcePath = BindingItemsSourcePath;
                bindableListView.ItemTemplate = ItemTemplate;
            }
        }
#else
        public new class UxmlTraits : ListView.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingItemsSourceAttribute = new()
                { name = "binding-items-source-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                visualElement
                    .As<BindableListView<TItemBindingContext, TCollection>>()
                    .BindingItemsSourcePath = _bindingItemsSourceAttribute.GetValueFromBag(bag, context);
            }
        }
#endif
    }
}