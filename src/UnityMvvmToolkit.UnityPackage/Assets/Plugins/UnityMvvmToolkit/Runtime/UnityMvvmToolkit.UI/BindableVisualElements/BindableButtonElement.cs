using System;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace UnityMvvmToolkit.UI.BindableVisualElements
{
    public abstract class BindableButtonElement : BindableCommandElement, IDisposable
    {
        private readonly BindableButton _button;
        private readonly ICommandWrapper _commandWrapper;

        protected BindableButtonElement(BindableButton button, IObjectProvider objectProvider)
            : base(objectProvider)
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