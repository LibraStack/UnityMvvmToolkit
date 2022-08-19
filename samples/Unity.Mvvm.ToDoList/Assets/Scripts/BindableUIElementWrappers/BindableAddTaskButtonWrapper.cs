using System;
using BindableUIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElementWrappers
{
    public class BindableAddTaskButtonWrapper : BindablePropertyElement, IInitializable, IDisposable
    {
        private readonly BindableAddTaskButton _addTaskButton;
        private readonly ICommand _command;
        private readonly IReadOnlyProperty<bool> _isCancelStateProperty;

        public BindableAddTaskButtonWrapper(BindableAddTaskButton addTaskButton, IObjectProvider objectProvider)
            : base(objectProvider)
        {
            _addTaskButton = addTaskButton;

            _command = GetCommand<ICommand>(_addTaskButton.Command);
            _isCancelStateProperty = GetReadOnlyProperty<bool>(_addTaskButton.BindingIsCancelStatePath);
        }

        public bool CanInitialize => _command != null && _isCancelStateProperty != null;

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
            _command.Execute();
        }
    }
}