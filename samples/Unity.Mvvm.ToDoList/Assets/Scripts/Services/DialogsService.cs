using Interfaces;
using Views;

namespace Services
{
    public class DialogsService : IDialogsService
    {
        private readonly MainView _main;
        private readonly AddTaskDialogView _addTaskDialogView;

        public DialogsService(IAppContext appContext)
        {
            _main = appContext.Resolve<MainView>();
            _addTaskDialogView = appContext.Resolve<AddTaskDialogView>();
        }

        public bool IsAddTaskDialogActive => _addTaskDialogView.IsDialogActive;

        public void ShowAddTaskDialog()
        {
            _main.ActivateBlocker();
            _addTaskDialogView.ShowDialog();
        }

        public void HideAddTaskDialog()
        {
            _main.DeactivateBlocker();
            _addTaskDialogView.HideDialog();
            _addTaskDialogView.BindingContext.TaskName = string.Empty;
        }
    }
}