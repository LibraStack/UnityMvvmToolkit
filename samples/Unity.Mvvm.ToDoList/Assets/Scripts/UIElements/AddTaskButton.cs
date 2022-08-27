using System;
using UnityEngine.UIElements;

namespace UIElements
{
    public class AddTaskButton : Button
    {
        private const string CancelStateClassName = "add-task-button--cancel";

        private bool _isAddTaskState = true;

        public AddTaskButton()
        {
            clicked += OnClicked;
        }

        public event EventHandler AddTask;
        public event EventHandler Cancel;

        public void ResetState()
        {
            _isAddTaskState = true;
            RemoveFromClassList(CancelStateClassName);
        }

        private void OnClicked()
        {
            if (_isAddTaskState)
            {
                AddToClassList(CancelStateClassName);
                AddTask?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                RemoveFromClassList(CancelStateClassName);
                Cancel?.Invoke(this, EventArgs.Empty);
            }

            _isAddTaskState = !_isAddTaskState;
        }

        public new class UxmlFactory : UxmlFactory<AddTaskButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : Button.UxmlTraits
        {
        }
    }
}