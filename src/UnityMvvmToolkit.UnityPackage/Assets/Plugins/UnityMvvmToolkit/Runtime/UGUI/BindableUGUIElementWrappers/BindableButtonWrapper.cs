using System;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UGUI.BindableUGUIElements;

namespace UnityMvvmToolkit.UGUI.BindableUGUIElementWrappers
{
    public class BindableButtonWrapper : BindableCommandElement, IDisposable
    {
        private readonly BindableButton _button;
        private readonly ICommandWrapper _commandWrapper;

        public BindableButtonWrapper(BindableButton button, IObjectProvider objectProvider) : base(objectProvider)
        {
            _commandWrapper = GetCommandWrapper(button.Command);

            if (_commandWrapper == null)
            {
                return;
            }

            _button = button;
            _button.Click += OnButtonClicked;
        }

        public void Dispose()
        {
            if (_commandWrapper != null)
            {
                _button.Click -= OnButtonClicked;
            }
        }

        private void OnButtonClicked()
        {
            _commandWrapper.Execute();
        }
    }
}