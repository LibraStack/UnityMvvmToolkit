using Cysharp.Threading.Tasks;
using Interfaces;
using Interfaces.Services;
using Views;

namespace Services
{
    public class DialogsService : IDialogsService
    {
        private readonly AddTaskDialogView _addTaskDialogView;

        public DialogsService(IAppContext appContext)
        {
            _addTaskDialogView = appContext.Resolve<AddTaskDialogView>();
        }

        public async UniTask ShowAddTaskDialogAsync()
        {
            await _addTaskDialogView.ShowDialogAsync();
        }

        public async UniTask HideAddTaskDialogAsync()
        {
            await _addTaskDialogView.HideDialogAsync();
        }
    }
}