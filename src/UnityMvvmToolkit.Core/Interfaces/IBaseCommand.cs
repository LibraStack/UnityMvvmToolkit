using System;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IBaseCommand
    {
        event EventHandler<bool> CanExecuteChanged;

        bool CanExecute();
        void RaiseCanExecuteChanged();
    }
}