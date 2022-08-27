using Cysharp.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IDialogsService
    {
        UniTask ShowAddTaskDialogAsync();
        UniTask HideAddTaskDialogAsync();
    }
}