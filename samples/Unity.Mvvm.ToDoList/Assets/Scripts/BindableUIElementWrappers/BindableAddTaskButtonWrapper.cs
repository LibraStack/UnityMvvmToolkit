using System;
using BindableUIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UniTask.Interfaces;

namespace BindableUIElementWrappers
{
    public class BindableAddTaskButtonWrapper : BindablePropertyElement, IInitializable, IDisposable
    {
        private readonly BindableAddTaskButton _addTaskButton;
        private readonly IAsyncCommand _asyncCommand;
        private readonly IReadOnlyProperty<bool> _isCancelStateProperty;

        public BindableAddTaskButtonWrapper(BindableAddTaskButton addTaskButton, IObjectProvider objectProvider)
            : base(objectProvider)
        {
            _addTaskButton = addTaskButton;

            _asyncCommand = GetCommand<IAsyncCommand>(_addTaskButton.Command);
            _isCancelStateProperty = GetReadOnlyProperty<bool>(_addTaskButton.BindingIsCancelStatePath);
        }

        public bool CanInitialize => _asyncCommand != null && _isCancelStateProperty != null;

        public void Initialize()
        {
            _addTaskButton.clicked += OnButtonClicked;
        }

        public override void UpdateValues()
        {
            _addTaskButton.SetState(_isCancelStateProperty.Value);
        }

        public void Dispose()
        {
            _addTaskButton.clicked -= OnButtonClicked;
        }

        private void OnButtonClicked()
        {
            _asyncCommand.Execute();
        }
    }
}