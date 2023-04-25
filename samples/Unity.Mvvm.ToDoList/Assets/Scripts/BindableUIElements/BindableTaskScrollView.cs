using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.BindableUIElements;
using ViewModels;

namespace BindableUIElements
{
    public partial class BindableTaskScrollView : BindableScrollView<TaskItemViewModel>
    {
        public override void Initialize()
        {
            base.Initialize();

            contentViewport.style.overflow = Overflow.Visible;
            contentContainer.style.overflow = Overflow.Visible;
        }
    }
}