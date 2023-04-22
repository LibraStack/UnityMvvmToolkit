using UnityEngine.UIElements;

namespace BindableUIElements
{
    public partial class BindableAddTaskButton
    {
        public new class UxmlFactory : UxmlFactory<BindableAddTaskButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableBinaryStateButton.UxmlTraits
        {
        }
    }
}