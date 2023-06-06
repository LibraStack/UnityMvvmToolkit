using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BaseButton
    {
#if UNITY_2023_2_OR_NEWER
        [System.Serializable]
        public new class UxmlSerializedData : Button.UxmlSerializedData
        {
            // ReSharper disable once InconsistentNaming
            #pragma warning disable 649
            [UnityEngine.SerializeField] private bool Enabled;
            #pragma warning restore 649

            public override void Deserialize(object visualElement)
            {
                base.Deserialize(visualElement);
                visualElement.As<BaseButton>().Enabled = Enabled;
            }
        }
#else
        public new class UxmlTraits : Button.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription _enabledAttribute = new()
                { name = "enabled", defaultValue = true };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                visualElement.As<BaseButton>().Enabled = _enabledAttribute.GetValueFromBag(bag, context);
            }
        }
#endif
    }
}