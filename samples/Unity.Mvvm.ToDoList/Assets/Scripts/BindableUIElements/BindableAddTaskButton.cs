using UIElements;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElements
{
    public class BindableAddTaskButton : AddTaskButton, IBindableUIElement
    {
        public string AddCommand { get; set; }
        public string CancelCommand { get; set; }

        public new class UxmlFactory : UxmlFactory<BindableAddTaskButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : AddTaskButton.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _addCommandAttribute = new()
                { name = "add-command", defaultValue = "" };

            private readonly UxmlStringAttributeDescription _cancelCommandAttribute = new()
                { name = "cancel-command", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);

                var bindableButton = (BindableAddTaskButton) visualElement;
                bindableButton.AddCommand = _addCommandAttribute.GetValueFromBag(bag, context);
                bindableButton.CancelCommand = _cancelCommandAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}