using Cysharp.Threading.Tasks;

namespace Interfaces
{
    public interface IDialogsService
    {
        UniTask ShowAddTaskDialogAsync();
        UniTask HideAddTaskDialogAsync();
    }
}