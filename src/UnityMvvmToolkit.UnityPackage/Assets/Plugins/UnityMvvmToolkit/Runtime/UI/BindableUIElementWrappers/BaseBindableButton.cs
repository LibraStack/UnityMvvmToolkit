using System;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace UnityMvvmToolkit.UI.BindableUIElementWrappers
{
    public abstract class BaseBindableButton : BindableCommandElement, IInitializable, IDisposable
    {
        private readonly BindableButton _button;
        private readonly ICommandWrapper _commandWrapper;

        protected BaseBindableButton(BindableButton button, IObjectProvider objectProvider) : base(objectProvider)
        {
            _button = button;
            _commandWrapper = GetCommandWrapper(button.Command);
        }

        public bool CanInitialize => _commandWrapper != null;

        public void Initialize()
        {
            _button.clicked += OnButtonClicked;
            _button.SetEnabled(_commandWrapper.CanExecute());
            _commandWrapper.CanExecuteChanged += OnCommandCanExecuteChanged;
        }

        public void Dispose()
        {
            _button.clicked -= OnButtonClicked;
            _commandWrapper.CanExecuteChanged -= OnCommandCanExecuteChanged;
        }

        private void OnButtonClicked()
        {
            _commandWrapper.Execute();
        }

        private void OnCommandCanExecuteChanged(object sender, bool canExecute)
        {
            _button.SetEnabled(canExecute);
        }
    }
}