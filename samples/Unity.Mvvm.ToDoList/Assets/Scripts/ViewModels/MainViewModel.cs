using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extensions;
using Interfaces;
using Interfaces.Services;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UniTask;
using UnityMvvmToolkit.UniTask.Interfaces;

namespace ViewModels
{
    public class MainViewModel : IBindingContext, IDisposable
    {
        private readonly IDialogsService _dialogsService;

        private readonly IProperty<string> _date;
        private readonly IProperty<int> _createdTasks;
        private readonly IProperty<int> _completedTasks;

        private readonly IProperty<bool> _isAddTaskDialogActive;

        private readonly IReadOnlyProperty<ObservableCollection<ICollectionItem>> _taskItems;

        public MainViewModel(IAppContext appContext, VisualTreeAsset taskItemTemplate)
        {
            _dialogsService = appContext.Resolve<IDialogsService>();

            _taskItems =
                new ObservableProperty<ObservableCollection<ICollectionItem>>(
                    new ObservableCollection<ICollectionItem>());
            _taskItems.Value.CollectionChanged += OnTaskItemsCollectionChanged;
            ChangeAddTaskDialogVisibilityCommand = new AsyncCommand(ChangeAddTaskDialogVisibility);

            _date = new ObservableProperty<string>(GetTodayDate());
            _createdTasks = new ObservableProperty<int>(GetCreatedTasksCount(_taskItems.Value));
            _completedTasks = new ObservableProperty<int>(GetCompletedTasksCount(_taskItems.Value));

            _isAddTaskDialogActive = new ObservableProperty<bool>();

            TaskItemTemplate = new ObservableProperty<VisualTreeAsset>(taskItemTemplate);

            SubscribeOnTaskAddMessage(appContext.Resolve<TaskBroker>()).Forget();
        }

        public string Date => _date.Value;
        public int CreatedTasks => _createdTasks.Value;
        public int CompletedTasks => _completedTasks.Value;
        public IReadOnlyCollection<ICollectionItem> TaskItems => _taskItems.Value;

        public IReadOnlyProperty<VisualTreeAsset> TaskItemTemplate { get; }

        public bool IsAddTaskDialogActive
        {
            get => _isAddTaskDialogActive.Value;
            set => _isAddTaskDialogActive.Value = value;
        }

        public IAsyncCommand ChangeAddTaskDialogVisibilityCommand { get; }

        public void Dispose()
        {
            _taskItems.Value.CollectionChanged -= OnTaskItemsCollectionChanged;
        }

        private async UniTask ChangeAddTaskDialogVisibility(CancellationToken cancellationToken = default)
        {
            IsAddTaskDialogActive = !IsAddTaskDialogActive;

            if (IsAddTaskDialogActive)
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
            // await foreach (var task in taskBroker.Subscribe())
            // {
            //     _taskItems/ValueTaskAwaiter<>.Add(new TaskItemViewModel { Name = task });
            //     await ChangeAddTaskDialogVisibility();
            // }
        }

        private string GetTodayDate()
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.Today.ToString("dddd, d MMM"));
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetCreatedTasksCount(ICollection taskItems)
        {
            return taskItems.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetCompletedTasksCount(IEnumerable<ICollectionItem> taskItems)
        {
            return taskItems.Count(item => ((TaskItemViewModel) item).IsDone);
        }

        public void AddTasks(IEnumerable<ICollectionItem> taskItems)
        {
            foreach (var taskItem in taskItems)
            {
                AddTask((TaskItemViewModel) taskItem);
            }
        }

        private void OnTaskItemIsDoneChanged(object sender, bool isDone)
        {
            UpdateTask((TaskItemViewModel) sender);
        }

        private void OnTaskItemRemoveClick(object sender, EventArgs e)
        {
            RemoveTask((TaskItemViewModel) sender);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddTask(TaskItemViewModel taskItem)
        {
            taskItem.RemoveClick += OnTaskItemRemoveClick;
            taskItem.IsDoneChanged += OnTaskItemIsDoneChanged;

            _taskItems.Value.Add(taskItem);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateTask(TaskItemViewModel taskItem)
        {
            _taskItems.Value.Update(taskItem);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveTask(TaskItemViewModel taskItem)
        {
            taskItem.RemoveClick -= OnTaskItemRemoveClick;
            taskItem.IsDoneChanged -= OnTaskItemIsDoneChanged;

            _taskItems.Value.Remove(taskItem);
        }
    }
}