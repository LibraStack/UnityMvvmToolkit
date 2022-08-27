using System;
using BindableUIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UniTask.Interfaces;

namespace BindableUIElementWrappers
{
    public class BindableAddTaskButtonWrapper : BindableCommandElement, IInitializable, IDisposable
    {
        private readonly BindableAddTaskButton _addTaskButton;
        private readonly IAsyncCommand _addTaskCommand;
        private readonly IAsyncCommand _cancelCommand;

        public BindableAddTaskButtonWrapper(BindableAddTaskButton addTaskButton, IObjectProvider objectProvider)
            : base(objectProvider)
        {
            _addTaskButton = addTaskButton;

            _addTaskCommand = GetCommand<IAsyncCommand>(addTaskButton.AddCommand);
            _cancelCommand = GetCommand<IAsyncCommand>(addTaskButton.CancelCommand);
        }

        public bool CanInitialize => _addTaskCommand != null && _cancelCommand != null;

        public void Initialize()
        {
            _addTaskButton.AddTask += OnAddTaskClicked;
            _addTaskButton.Cancel += OnCancelClicked;
        }

        public void Dispose()
        {
            _addTaskButton.AddTask -= OnAddTaskClicked;
            _addTaskButton.Cancel -= OnCancelClicked;
        }

        private void OnAddTaskClicked(object sender, EventArgs e)
        {
            _addTaskCommand.Execute();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            _cancelCommand.Execute();
        }
    }
}