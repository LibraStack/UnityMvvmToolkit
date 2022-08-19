using Cysharp.Threading.Tasks;

namespace Interfaces
{
    public interface IDialogsService
    {
        bool IsAddTaskDialogActive { get; }
        
        UniTask ShowAddTaskDialogAsync();
        UniTask HideAddTaskDialogAsync();
    }
}