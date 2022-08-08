using System;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace UnityMvvmToolkit.UI.BindableUIElementWrappers
{
    public abstract class BaseBindableButton : BindableCommandElement, IDisposable
    {
        private readonly BindableButton _button;
        private readonly ICommandWrapper _commandWrapper;

        protected BaseBindableButton(BindableButton button, IObjectProvider objectProvider) : base(objectProvider)
        {
            _commandWrapper = GetCommandWrapper(button.Command);

            if (_commandWrapper == null)
            {
                return;
            }

            _button = button;
            _button.clicked += OnButtonClicked;
        }

        public void Dispose()
        {
            if (_commandWrapper != null)
            {
                _button.clicked -= OnButtonClicked;
            }
        }

        private void OnButtonClicked()
        {
            _commandWrapper.Execute();
        }
    }
}