using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindableLabel
    {
        public string BindingTextPath { get; private set; }

        public new class UxmlFactory : UxmlFactory<BindableLabel, UxmlTraits>
        {
        }

#if UNITY_2023_2_OR_NEWER
        [System.Serializable]
        public new class UxmlSerializedData : Label.UxmlSerializedData
        {
            // ReSharper disable once InconsistentNaming
            #pragma warning disable 649
            [UnityEngine.SerializeField] private string BindingTextPath;
            #pragma warning restore 649

            public override object CreateInstance() => new BindableLabel();
            public override void Deserialize(object visualElement)
            {
                base.Deserialize(visualElement);
                visualElement.As<BindableLabel>().BindingTextPath = BindingTextPath;
            }
        }
#else
        public new class UxmlTraits : Label.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingTextAttribute = new()
                { name = "binding-text-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                visualElement.As<BindableLabel>().BindingTextPath = _bindingTextAttribute.GetValueFromBag(bag, context);
            }
        }
#endif
    }
}