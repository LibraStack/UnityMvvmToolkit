using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindableDropdownField
    {
        public string BindingSelectedItemPath { get; private set; }

        public string BindingItemsSourcePath { get; private set; }

        public new class UxmlFactory : UxmlFactory<BindableDropdownField, UxmlTraits>
        {
        }
        
#if UNITY_2023_2_OR_NEWER
        [System.Serializable]
        public new class UxmlSerializedData : DropdownField.UxmlSerializedData
        {
            // ReSharper disable once InconsistentNaming
            #pragma warning disable 649
            [UnityEngine.SerializeField] private string BindingSelectedItemPath;
            [UnityEngine.SerializeField] private string BindingItemsSourcePath;
            #pragma warning restore 649

            public override object CreateInstance() => new BindableDropdownField();
            public override void Deserialize(object visualElement)
            {
                base.Deserialize(visualElement);

                var bindableDropdownField = visualElement.As<BindableDropdownField>();
                bindableDropdownField.BindingSelectedItemPath = BindingSelectedItemPath;
                bindableDropdownField.BindingItemsSourcePath = BindingItemsSourcePath;
            }
        }
#else
        public new class UxmlTraits : DropdownField.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingSelectedItemPath = new()
                { name = "binding-selected-item-path", defaultValue = "" };

            private readonly UxmlStringAttributeDescription _bindingItemsSourcePath = new()
                { name = "binding-items-source-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                var bindableDropdownField = visualElement.As<BindableDropdownField>();
                
                bindableDropdownField.BindingSelectedItemPath = _bindingSelectedItemPath.GetValueFromBag(bag, context);
                bindableDropdownField.BindingItemsSourcePath = _bindingItemsSourcePath.GetValueFromBag(bag, context);
            }
        }
#endif
    }
}