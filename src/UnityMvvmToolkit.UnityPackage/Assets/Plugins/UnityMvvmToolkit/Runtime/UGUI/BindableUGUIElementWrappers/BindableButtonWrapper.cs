using System;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UGUI.BindableUGUIElements;

namespace UnityMvvmToolkit.UGUI.BindableUGUIElementWrappers
{
    public class BindableButtonWrapper// : BindableCommandElement, IInitializable, IDisposable
    {
        private readonly int _buttonId;
        private readonly BindableButton _button;
        private readonly ICommandWrapper _commandWrapper;

        public BindableButtonWrapper(BindableButton button, IObjectProvider objectProvider)// : base(objectProvider)
        {
            _button = button;
            _buttonId = button.GetInstanceID();
            //_commandWrapper = GetCommandWrapper(_buttonId, button.Command);
        }

        public bool CanInitialize => _commandWrapper != null;

        public void Initialize()
        {
            _button.Click += OnButtonClicked;
        }

        public void Dispose()
        {
            _button.Click -= OnButtonClicked;
        }

        private void OnButtonClicked()
        {
            _commandWrapper.Execute(_buttonId);
        }
    }
}