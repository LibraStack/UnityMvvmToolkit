using UnityEngine.UIElements;

namespace BindableUIElements
{
    public class BindableAddTaskButton : BindableBinaryStateButton
    {
        private const string CancelStateClassName = "add-task-button--cancel";

        public override void Activate()
        {
            AddToClassList(CancelStateClassName);
        }

        public override void Deactivate()
        {
            RemoveFromClassList(CancelStateClassName);
        }

        public new class UxmlFactory : UxmlFactory<BindableAddTaskButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableBinaryStateButton.UxmlTraits
        {
        }
    }
}