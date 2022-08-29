using System;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace UnityMvvmToolkit.UITK.BindableUIElementWrappers
{
    public abstract class BaseBindableButton : BindableCommandElement, IInitializable, IDisposable
    {
        private readonly int _buttonId;
        private readonly BindableButton _button;
        private readonly ICommandWrapper _commandWrapper;

        protected BaseBindableButton(BindableButton button, IObjectProvider objectProvider) : base(objectProvider)
        {
            _button = button;
            _buttonId = button.GetHashCode();
            _commandWrapper = GetCommandWrapper(_buttonId, button.Command);
        }

        public bool CanInitialize => _commandWrapper != null;

        public void Initialize()
        {
            _button.clicked += OnButtonClicked;
            _button.Enabled = _commandWrapper.CanExecute();
            _commandWrapper.CanExecuteChanged += OnCommandCanExecuteChanged;
        }

        public void Dispose()
        {
            _button.clicked -= OnButtonClicked;
            _commandWrapper.CanExecuteChanged -= OnCommandCanExecuteChanged;
        }

        private void OnButtonClicked()
        {
            _commandWrapper.Execute(_buttonId);
        }

        private void OnCommandCanExecuteChanged(object sender, bool canExecute)
        {
            _button.Enabled = canExecute;
        }
    }
}