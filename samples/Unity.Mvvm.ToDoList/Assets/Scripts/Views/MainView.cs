using BindableUIElements;
using UnityEngine.UIElements;
using ViewModels;

namespace Views
{
    public class MainView : BaseView<MainViewModel>
    {
        private BindablePageBlocker _blocker;

        protected override void OnInit()
        {
            base.OnInit();
            _blocker = RootVisualElement.Q<BindablePageBlocker>("Blocker");
        }

        public void ActivateBlocker()
        {
            _blocker.Activate();
        }

        public void DeactivateBlocker()
        {
            _blocker.Deactivate();
        }
    }
}