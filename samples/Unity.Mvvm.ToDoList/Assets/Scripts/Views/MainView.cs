using BindableUIElements;
using Cysharp.Threading.Tasks;
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
            _blocker = RootVisualElement.Q<BindablePageBlocker>("Blocker"); // TODO: Move?
        }

        public async UniTask ActivateBlockerAsync()
        {
            await _blocker.ActivateAsync();
        }

        public async UniTask DeactivateBlockerAsync()
        {
            await _blocker.DeactivateAsync();
        }
    }
}