using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindableTextField
    {
        public string BindingValuePath { get; private set; }

        public new class UxmlFactory : UxmlFactory<BindableTextField, UxmlTraits>
        {
        }

#if UNITY_2023_2_OR_NEWER
        [System.Serializable]
        public new class UxmlSerializedData : TextField.UxmlSerializedData
        {
            // ReSharper disable once InconsistentNaming
            #pragma warning disable 649
            [UnityEngine.SerializeField] private string BindingValuePath;
            #pragma warning restore 649

            public override object CreateInstance() => new BindableTextField();
            public override void Deserialize(object visualElement)
            {
                base.Deserialize(visualElement);
                visualElement.As<BindableTextField>().BindingValuePath = BindingValuePath;
            }
        }
#else
        public new class UxmlTraits : TextField.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingValueAttribute = new()
                { name = "binding-value-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                visualElement
                    .As<BindableTextField>()
                    .BindingValuePath = _bindingValueAttribute.GetValueFromBag(bag, context);
            }
        }
#endif
    }
}