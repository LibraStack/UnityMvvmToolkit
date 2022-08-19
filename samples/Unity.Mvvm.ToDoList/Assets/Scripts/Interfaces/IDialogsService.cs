namespace Interfaces
{
    public interface IDialogsService
    {
        bool IsAddTaskDialogActive { get; }
        
        void ShowAddTaskDialog();
        void HideAddTaskDialog();
    }
}