using System;
using UnityEngine.UIElements;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace Controllers
{
    public class TaskItemController : IDisposable
    {
        private const string LabelDoneClassName = "task-item__label--done";
        private const string StateCheckDoneClassName = "task-item__state-check--done";
        private const string StateCircleDoneClassName = "task-item__state-circle--done";

        private readonly VisualElement _stateCheck;
        private readonly VisualElement _stateCircle;

        private readonly BindableLabel _titleLabel;
        private readonly BindableButton _stateButton;
        private readonly BindableButton _removeButton;

        private readonly Action<TaskItemData> _actionOnStateChanged;
        private readonly Action<TaskItemData> _actionOnRemoveClicked;

        private TaskItemData _taskItemData;

        public TaskItemController(VisualElement taskItemAsset, Action<TaskItemData> actionOnStateChanged = null,
            Action<TaskItemData> actionOnRemoveClicked = null)
        {
            _titleLabel = taskItemAsset.Q<BindableLabel>("TitleLabel");
            _stateCheck = taskItemAsset.Q<VisualElement>("Check");
            _stateCircle = taskItemAsset.Q<VisualElement>("Circle");

            _stateButton = taskItemAsset.Q<BindableButton>("StateButton");
            _stateButton.clicked += OnStateButtonClick;
            
            _removeButton = taskItemAsset.Q<BindableButton>("RemoveButton");
            _removeButton.clicked += OnRemoveButtonClick;

            _actionOnStateChanged = actionOnStateChanged;
            _actionOnRemoveClicked = actionOnRemoveClicked;
        }

        public void SetData(TaskItemData taskItemData)
        {
            _taskItemData = taskItemData;
            _titleLabel.text = taskItemData.Name;
            
            UpdateState();
        }

        public void Dispose()
        {
            _stateButton.clicked -= OnStateButtonClick;
            _removeButton.clicked -= OnRemoveButtonClick;
        }

        private void OnStateButtonClick()
        {
            _taskItemData.IsDone = !_taskItemData.IsDone;
            _actionOnStateChanged?.Invoke(_taskItemData);
            
            UpdateState();
        }

        private void OnRemoveButtonClick()
        {
            _actionOnRemoveClicked?.Invoke(_taskItemData);
        }

        private void UpdateState()
        {
            if (_taskItemData.IsDone)
            {
                _titleLabel.AddToClassList(LabelDoneClassName);
                _stateCheck.AddToClassList(StateCheckDoneClassName);
                _stateCircle.AddToClassList(StateCircleDoneClassName);
            }
            else
            {
                _titleLabel.RemoveFromClassList(LabelDoneClassName);
                _stateCheck.RemoveFromClassList(StateCheckDoneClassName);
                _stateCircle.RemoveFromClassList(StateCircleDoneClassName);
            }
        }
    }
}