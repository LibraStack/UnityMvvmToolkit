using Cysharp.Threading.Tasks;
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

        public async UniTask ShowAddTaskDialogAsync()
        {
            await UniTask.WhenAll(_main.ActivateBlockerAsync(), _addTaskDialogView.ShowDialogAsync());
        }

        public async UniTask HideAddTaskDialogAsync()
        {
            await UniTask.WhenAll(_main.DeactivateBlockerAsync(), _addTaskDialogView.HideDialogAsync());
            _addTaskDialogView.BindingContext.TaskName = string.Empty;
        }
    }
}