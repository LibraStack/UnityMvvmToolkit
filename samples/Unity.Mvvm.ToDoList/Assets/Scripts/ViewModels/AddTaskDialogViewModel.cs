using System;
using Interfaces;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace ViewModels
{
    public class AddTaskDialogViewModel : IBindingContext, IInitializable, IDisposable
    {
        private readonly TaskBroker _taskBroker;
        private readonly IProperty<string> _taskName;

        public AddTaskDialogViewModel(IAppContext appContext)
        {
            _taskName = new Property<string>();
            _taskBroker = appContext.Resolve<TaskBroker>();

            AddTaskCommand = new Command(AddTask, CanAddTask);
        }

        public string TaskName
        {
            get => _taskName.Value;
            set => _taskName.Value = value;
        }

        public ICommand AddTaskCommand { get; }

        public void Initialize()
        {
            _taskName.ValueChanged += OnTaskNameValueChanged;
        }

        public void Dispose()
        {
            _taskName.ValueChanged -= OnTaskNameValueChanged;
        }

        private bool CanAddTask() => string.IsNullOrWhiteSpace(TaskName) == false;

        private void AddTask()
        {
            _taskBroker.Publish(TaskName);
        }

        private void OnTaskNameValueChanged(object sender, string newValue)
        {
            AddTaskCommand.RaiseCanExecuteChanged();
        }
    }
}