using UnityEngine.UIElements;
using ViewModels;

namespace Views
{
    public class MainView : BaseView<MainViewModel>
    {
        protected override void OnInit()
        {
            base.OnInit();

            // TODO: Move to control.
            var scrollView = RootVisualElement.Q<ScrollView>();
            scrollView.contentViewport.style.overflow = Overflow.Visible;
            scrollView.contentContainer.style.overflow = Overflow.Visible;
        }
    }
}