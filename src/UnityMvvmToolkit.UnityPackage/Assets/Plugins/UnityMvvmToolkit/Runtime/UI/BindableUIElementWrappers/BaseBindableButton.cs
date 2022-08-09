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
        }

        public void Dispose()
        {
            _button.clicked -= OnButtonClicked;
        }

        private void OnButtonClicked()
        {
            _commandWrapper.Execute();
        }
    }
}