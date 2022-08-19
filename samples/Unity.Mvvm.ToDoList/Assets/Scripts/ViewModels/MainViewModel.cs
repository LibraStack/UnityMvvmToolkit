using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Cysharp.Threading.Tasks;
using Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace ViewModels
{
    public class MainViewModel : ViewModel, IDisposable
    {
        private readonly IDialogsService _dialogsService;

        public MainViewModel(IAppContext appContext)
        {
            _dialogsService = appContext.Resolve<IDialogsService>();

            TaskItems = new ObservableCollection<TaskItemData>
            {
                new("Add UnitTests"),
                new("Create UGUI ListView"),
                new("Write an article"),
                new("Add UI Toolkit ListView"),
                new("Fix Command bindings")
            };
            TaskItems.CollectionChanged += OnTaskItemsCollectionChanged;

            ShowAddTaskDialogCommand = new Command(SetAddTaskDialogActive);
            HideAddTaskDialogCommand = new Command(HideAddTaskDialog, IsAddTaskDialogActive);

            SubscribeOnTaskAddMessage(appContext.Resolve<TaskBroker>()).Forget();
        }

        public string Date => GetTodayDate();
        public int CreatedTasks => TaskItems.Count;
        public int CompletedTasks => TaskItems.Count(data => data.IsDone);
        public bool IsAddTaskButtonCancelState => IsAddTaskDialogActive();
        public ObservableCollection<TaskItemData> TaskItems { get; }

        public ICommand ShowAddTaskDialogCommand { get; }
        public ICommand HideAddTaskDialogCommand { get; }

        private bool IsAddTaskDialogActive() => _dialogsService.IsAddTaskDialogActive;

        public void Dispose()
        {
            TaskItems.CollectionChanged -= OnTaskItemsCollectionChanged;
        }

        private void SetAddTaskDialogActive()
        {
            if (IsAddTaskDialogActive())
            {
                HideAddTaskDialog();
            }
            else
            {
                ShowAddTaskDialog();
            }
        }

        private void ShowAddTaskDialog()
        {
            _dialogsService.ShowAddTaskDialog();
            HideAddTaskDialogCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(IsAddTaskButtonCancelState));
        }

        private void HideAddTaskDialog()
        {
            _dialogsService.HideAddTaskDialog();
            HideAddTaskDialogCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(IsAddTaskButtonCancelState));
        }

        private async UniTaskVoid SubscribeOnTaskAddMessage(TaskBroker taskBroker)
        {
            await foreach (var task in taskBroker.Subscribe())
            {
                TaskItems.Add(new TaskItemData(task));
                HideAddTaskDialog();
            }
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
                    OnPropertyChanged(nameof(CreatedTasks));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    OnPropertyChanged(nameof(CompletedTasks));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OnPropertyChanged(nameof(CreatedTasks));
                    OnPropertyChanged(nameof(CompletedTasks));
                    break;
            }
        }
    }
}