using Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace ViewModels
{
    public class AddTaskDialogViewModel : ViewModel
    {
        private readonly TaskBroker _taskBroker;

        private string _taskName;
        private string _keyboardHeight;

        public AddTaskDialogViewModel(IAppContext appContext)
        {
            _taskBroker = appContext.Resolve<TaskBroker>();
            AddTaskCommand = new Command(AddTask, CanAddTask);
        }

        public string TaskName
        {
            get => _taskName;
            set
            {
                if (Set(ref _taskName, value))
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