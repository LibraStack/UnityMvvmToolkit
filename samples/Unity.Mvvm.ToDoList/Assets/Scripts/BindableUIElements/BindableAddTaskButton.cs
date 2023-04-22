namespace BindableUIElements
{
    public partial class BindableAddTaskButton : BindableBinaryStateButton
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
    }
}