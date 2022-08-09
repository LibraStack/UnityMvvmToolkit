#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.Interfaces
{
    public interface IBaseAsyncCommand
    {
        bool DisableOnExecution { get; set; }
    }
}

#endif