#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.Interfaces
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityMvvmToolkit.Core.Interfaces;

    public interface IAsyncCommand<in T> : IBaseAsyncCommand, ICommand<T>
    {
        UniTask ExecuteAsync(T parameter, CancellationToken cancellationToken = default);
    }
}

#endif