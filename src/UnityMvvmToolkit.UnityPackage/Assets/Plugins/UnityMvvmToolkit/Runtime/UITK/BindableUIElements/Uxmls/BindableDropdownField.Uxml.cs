using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public partial class BindableDropdownField
    {
        public string BindingValuePath { get; private set; }

        public string BindingChoicesPath { get; private set; }


        public new class UxmlFactory : UxmlFactory<BindableDropdownField, UxmlTraits>
        {
        }
        
#if UNITY_2023_2_OR_NEWER
        [System.Serializable]
        public new class UxmlSerializedData : DropdownField.UxmlSerializedData
        {
            // ReSharper disable once InconsistentNaming
            #pragma warning disable 649
            [UnityEngine.SerializeField] private string BindingValuePath;
            [UnityEngine.SerializeField] private string BindingItemsSourcePath;
            #pragma warning restore 649

            public override object CreateInstance() => new BindableDropdownField();
            public override void Deserialize(object visualElement)
            {
                base.Deserialize(visualElement);
                visualElement.As<BindableDropdownField>().BindingValuePath = BindingValuePath;
                visualElement.As<BindableDropdownField>().BindingItemsSourcePath = BindingItemsSourcePath;
            }
        }
#else
        public new class UxmlTraits : DropdownField.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingValueAttribute = new()
                { name = "binding-value-path", defaultValue = "" };

            private readonly UxmlStringAttributeDescription _bindingChoicesAttribute = new()
                { name = "binding-choices-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                visualElement
                    .As<BindableDropdownField>()
                    .BindingValuePath = _bindingValueAttribute.GetValueFromBag(bag, context);
                
                visualElement
                    .As<BindableDropdownField>()
                    .BindingChoicesPath = _bindingChoicesAttribute.GetValueFromBag(bag, context);
            }
        }
#endif
        
    }
}