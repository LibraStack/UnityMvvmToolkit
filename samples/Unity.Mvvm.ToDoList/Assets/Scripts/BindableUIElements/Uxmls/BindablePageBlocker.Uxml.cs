using UnityEngine.UIElements;

namespace BindableUIElements
{
    public partial class BindablePageBlocker
    {
        public new class UxmlFactory : UxmlFactory<BindablePageBlocker, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableBinaryStateButton.UxmlTraits
        {
        }
    }
}