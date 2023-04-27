using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extensions;
using Interfaces;
using Interfaces.Services;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UniTask;
using UnityMvvmToolkit.UniTask.Interfaces;

// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ViewModels
{
    public class MainViewModel : IBindingContext, IDisposable
    {
        [Observable("Date")]
        private readonly IReadOnlyProperty<string> _date;

        [Observable("CreatedTasks")]
        private readonly IProperty<int> _createdTasks;

        [Observable("CompletedTasks")]
        private readonly IProperty<int> _completedTasks;

        [Observable("IsAddTaskDialogActive")]
        private readonly IProperty<bool> _isAddTaskDialogActive;

        [Observable("TaskItems")]
        private readonly IReadOnlyProperty<ObservableCollection<TaskItemViewModel>> _taskItems;

        private readonly IDialogsService _dialogsService;

        public MainViewModel(IAppContext appContext)
        {
            _dialogsService = appContext.Resolve<IDialogsService>();

            _taskItems =
                new ReadOnlyProperty<ObservableCollection<TaskItemViewModel>>(
                    new ObservableCollection<TaskItemViewModel>());
            _taskItems.Value.CollectionChanged += OnTaskItemsCollectionChanged;

            _date = new ReadOnlyProperty<string>(GetTodayDate());
            _createdTasks = new Property<int>(GetCreatedTasksCount(_taskItems.Value));
            _completedTasks = new Property<int>(GetCompletedTasksCount(_taskItems.Value));

            _isAddTaskDialogActive = new Property<bool>();

            ChangeAddTaskDialogVisibilityCommand = new AsyncCommand(ChangeAddTaskDialogVisibility);

            SubscribeOnTaskAddMessage(appContext.Resolve<TaskBroker>()).Forget();
        }

        public IAsyncCommand ChangeAddTaskDialogVisibilityCommand { get; }

        public event EventHandler<NotifyCollectionChangedEventArgs> TaskItemsChanged;

        public IEnumerable<TaskItemViewModel> GetTaskItems()
        {
            return _taskItems.Value;
        }

        public void AddTasks(IEnumerable<ICollectionItem> taskItems)
        {
            foreach (var taskItem in taskItems)
            {
                AddTask((TaskItemViewModel) taskItem);
            }
        }

        public void Dispose()
        {
            var taskItems = _taskItems.Value;

            for (var i = 0; i < taskItems.Count; i++)
            {
                RemoveTask(taskItems[i], false);
            }

            _taskItems.Value.CollectionChanged -= OnTaskItemsCollectionChanged;
            _taskItems.Value.Clear();
        }

        private async UniTask ChangeAddTaskDialogVisibility(CancellationToken cancellationToken = default)
        {
            _isAddTaskDialogActive.Value = !_isAddTaskDialogActive.Value;

            if (_isAddTaskDialogActive.Value)
            {
                await _dialogsService.ShowAddTaskDialogAsync();
            }
            else
            {
                await _dialogsService.HideAddTaskDialogAsync();
            }
        }

        private async UniTaskVoid SubscribeOnTaskAddMessage(TaskBroker taskBroker)
        {
            await foreach (var newTask in taskBroker.Subscribe())
            {
                AddTask(new TaskItemViewModel { Name = newTask });
                await ChangeAddTaskDialogVisibility();
            }
        }

        private void OnTaskItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    _createdTasks.Value = GetCreatedTasksCount(_taskItems.Value);
                    _completedTasks.Value = GetCompletedTasksCount(_taskItems.Value);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    _completedTasks.Value = GetCompletedTasksCount(_taskItems.Value);
                    break;
            }

            TaskItemsChanged?.Invoke(this, e);
        }

        private static string GetTodayDate()
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Today.ToString("dddd, d MMM"));
        }

        private static int GetCreatedTasksCount(ICollection taskItems)
        {
            return taskItems.Count;
        }

        private static int GetCompletedTasksCount(IEnumerable<ICollectionItem> taskItems)
        {
            return taskItems.Count(item => ((TaskItemViewModel) item).IsDone);
        }

        private void AddTask(TaskItemViewModel taskItem)
        {
            taskItem.RemoveClick += OnTaskItemRemoveClick;
            taskItem.IsDoneChanged += OnTaskItemIsDoneChanged;

            _taskItems.Value.Add(taskItem);
        }

        private void UpdateTask(TaskItemViewModel taskItem)
        {
            _taskItems.Value.Update(taskItem);
        }

        private void RemoveTask(TaskItemViewModel taskItem, bool removeFromCollection)
        {
            if (removeFromCollection)
            {
                _taskItems.Value.Remove(taskItem);
            }

            taskItem.RemoveClick -= OnTaskItemRemoveClick;
            taskItem.IsDoneChanged -= OnTaskItemIsDoneChanged;
        }

        private void OnTaskItemIsDoneChanged(object sender, bool isDone)
        {
            UpdateTask((TaskItemViewModel) sender);
        }

        private void OnTaskItemRemoveClick(object sender, EventArgs e)
        {
            RemoveTask((TaskItemViewModel) sender, true);
        }
    }
}