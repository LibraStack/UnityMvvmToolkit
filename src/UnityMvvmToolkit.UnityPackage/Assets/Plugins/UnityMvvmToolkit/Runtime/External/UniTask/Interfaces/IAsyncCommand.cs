#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.Interfaces
{
    using UnityMvvmToolkit.Common.Interfaces;

    public interface IAsyncCommand : ICommand
    {
    }

    public interface IAsyncCommand<in T> : ICommand<T>
    {
    }
}

#endif