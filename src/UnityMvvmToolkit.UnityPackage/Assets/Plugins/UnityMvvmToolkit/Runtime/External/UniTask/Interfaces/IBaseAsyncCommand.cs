#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.Interfaces
{
    using UnityMvvmToolkit.Core.Interfaces;

    public interface IBaseAsyncCommand
    {
        bool AllowConcurrency { get; set; }
        bool DisableOnExecution { get; set; }

        IReadOnlyProperty<bool> IsRunning { get; }

        void Cancel();
    }
}

#endif