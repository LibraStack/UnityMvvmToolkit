using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindableButton
    {
        public string Command { get; private set; }

        public string BindingTextPath { get; private set; }

        public new class UxmlFactory : UxmlFactory<BindableButton, UxmlTraits>
        {
        }

#if UNITY_2023_2_OR_NEWER
        [System.Serializable]
        public new class UxmlSerializedData : BaseButton.UxmlSerializedData
        {
            // ReSharper disable once InconsistentNaming
            #pragma warning disable 649
            [UnityEngine.SerializeField] private string Command;
#pragma warning restore 649

            [UnityEngine.SerializeField] private string BindingTextPath;

            public override object CreateInstance() => new BindableButton();

            public override void Deserialize(object visualElement)
            {
                base.Deserialize(visualElement);
                visualElement.As<BindableButton>().Command = Command;
                visualElement.As<BindableButton>().BindingTextPath = BindingTextPath;
            }
        }
#else
        public new class UxmlTraits : BaseButton.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _commandAttribute = new()
            {
                name = "command", defaultValue = ""
            };

            private readonly UxmlStringAttributeDescription _bindingTextAttribute = new()
            {
                name = "binding-text-path", defaultValue = ""
            };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                visualElement.As<BindableButton>().Command = _commandAttribute.GetValueFromBag(bag, context);
                visualElement.As<BindableButton>().BindingTextPath = _bindingTextAttribute.GetValueFromBag(bag, context);
            }
        }
#endif
    }
}