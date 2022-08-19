using System;
using Extensions;
using UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ViewModels;

namespace Views
{
    public class AddTaskDialogView : BaseView<AddTaskDialogViewModel>, IDisposable
    {
        private MobileInputAdaptivePage _mobileInputAdaptivePage;

        public bool IsDialogActive => RootVisualElement?.visible ?? false;
        
        protected override void OnInit()
        {
            base.OnInit();
            _mobileInputAdaptivePage = RootVisualElement.Q<MobileInputAdaptivePage>();
        }

        public void ShowDialog()
        {
            _mobileInputAdaptivePage.Activate();
        }

        public void HideDialog()
        {
            _mobileInputAdaptivePage.Deactivate();
        }

        public void Dispose()
        {
            _mobileInputAdaptivePage?.Dispose();
        }
    }
}