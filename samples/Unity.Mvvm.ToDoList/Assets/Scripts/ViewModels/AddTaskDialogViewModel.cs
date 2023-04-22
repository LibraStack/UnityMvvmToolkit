using Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace ViewModels
{
    public class AddTaskDialogViewModel : IBindingContext
    {
        private readonly TaskBroker _taskBroker;
        private readonly IProperty<string> _taskName;

        public AddTaskDialogViewModel(IAppContext appContext)
        {
            _taskBroker = appContext.Resolve<TaskBroker>();
            _taskName = new ObservableProperty<string>();

            AddTaskCommand = new Command(AddTask, CanAddTask);
        }

        public string TaskName
        {
            get => _taskName.Value;
            set
            {
                if (_taskName.TrySetValue(value))
                {
                    AddTaskCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand AddTaskCommand { get; }

        private bool CanAddTask() => string.IsNullOrWhiteSpace(TaskName) == false;

        private void AddTask()
        {
            _taskBroker.Publish(TaskName);
        }
    }
}