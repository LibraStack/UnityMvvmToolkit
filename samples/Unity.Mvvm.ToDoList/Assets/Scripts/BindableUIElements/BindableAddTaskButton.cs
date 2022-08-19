using UnityEngine.UIElements;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace BindableUIElements
{
    public class BindableAddTaskButton : BindableButton
    {
        private const string CancelStateClassName = "add-task-button--cancel";

        public string BindingIsCancelStatePath { get; set; }

        public void SetState(bool isCancelState)
        {
            if (isCancelState)
            {
                AddToClassList(CancelStateClassName);
            }
            else
            {
                RemoveFromClassList(CancelStateClassName);
            }
        }

        public new class UxmlFactory : UxmlFactory<BindableAddTaskButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableButton.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _isCancelStateAttribute = new()
                { name = "binding-is-cancel-state-path", defaultValue = string.Empty };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((BindableAddTaskButton) visualElement).BindingIsCancelStatePath =
                    _isCancelStateAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}