using System;
using Cysharp.Threading.Tasks;
using UIElements;
using UnityEngine.UIElements;
using ViewModels;

namespace Views
{
    public class AddTaskDialogView : BaseView<AddTaskDialogViewModel>, IDisposable
    {
        private MobileInputAdaptivePage _mobileInputAdaptivePage;
        
        protected override void OnInit()
        {
            base.OnInit();
            _mobileInputAdaptivePage = RootVisualElement.Q<MobileInputAdaptivePage>();
        }

        public async UniTask ShowDialogAsync()
        {
            BindingContext.TaskName = string.Empty;
            await _mobileInputAdaptivePage.ActivateAsync();
        }

        public async UniTask HideDialogAsync()
        {
            await _mobileInputAdaptivePage.DeactivateAsync();
        }

        public void Dispose()
        {
            _mobileInputAdaptivePage?.Dispose();
        }
    }
}