using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace BindableUIElements
{
    public partial class BindableCountLabel
    {
        public new class UxmlFactory : UxmlFactory<BindableCountLabel, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableLabel.UxmlTraits
        {
        }
    }
}