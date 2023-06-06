using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindingContextProvider<TBindingContext>
    {
        public string BindingContextPath { get; protected set; }

#if UNITY_2023_2_OR_NEWER
        [System.Serializable]
        public new class UxmlSerializedData : VisualElement.UxmlSerializedData
        {
            // ReSharper disable once InconsistentNaming
            #pragma warning disable 649
            [UnityEngine.SerializeField] private string BindingContextPath;
            #pragma warning restore 649

            public override void Deserialize(object visualElement)
            {
                base.Deserialize(visualElement);

                visualElement
                    .As<BindingContextProvider<TBindingContext>>()
                    .BindingContextPath = BindingContextPath;
            }
        }
#else
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
#endif
    }
}