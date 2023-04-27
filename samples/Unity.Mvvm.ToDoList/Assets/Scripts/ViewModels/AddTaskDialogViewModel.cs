using Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace ViewModels
{
    public class AddTaskDialogViewModel : IBindingContext
    {
        private readonly TaskBroker _taskBroker;

        public AddTaskDialogViewModel(IAppContext appContext)
        {
            _taskBroker = appContext.Resolve<TaskBroker>();

            TaskName = new Property<string>();
            TaskName.ValueChanged += OnTaskNameValueChanged;

            AddTaskCommand = new Command(AddTask, CanAddTask);
        }

        public IProperty<string> TaskName { get; }

        public ICommand AddTaskCommand { get; }

        private bool CanAddTask() => string.IsNullOrWhiteSpace(TaskName.Value) == false;

        private void AddTask()
        {
            _taskBroker.Publish(TaskName.Value);
        }

        private void OnTaskNameValueChanged(object sender, string newValue)
        {
            AddTaskCommand.RaiseCanExecuteChanged();
        }
    }
}