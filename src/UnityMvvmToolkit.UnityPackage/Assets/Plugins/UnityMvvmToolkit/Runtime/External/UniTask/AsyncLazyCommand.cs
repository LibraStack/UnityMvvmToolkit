#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public class AsyncLazyCommand : AsyncCommand
    {
        public AsyncLazyCommand(Func<CancellationToken, UniTask> action, Func<bool> canExecute = null)
            : base(action, canExecute)
        {
        }

        public override async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (ExecuteTask?.Task.Status.IsCompleted() ?? true)
                {
                    ExecuteTask = Action.Invoke(cancellationToken).ToAsyncLazy();
                }

                await ExecuteTask;
            }
            finally
            {
                ExecuteTask = null;
            }
        }
    }

    public class AsyncLazyCommand<T> : AsyncCommand<T>
    {
        public AsyncLazyCommand(Func<T, CancellationToken, UniTask> action, Func<bool> canExecute = null)
            : base(action, canExecute)
        {
        }

        public override async UniTask ExecuteAsync(T parameter, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ExecuteTask?.Task.Status.IsCompleted() ?? true)
                {
                    ExecuteTask = Action.Invoke(parameter, cancellationToken).ToAsyncLazy();
                }

                await ExecuteTask;
            }
            finally
            {
                ExecuteTask = null;
            }
        }
    }
}

#endif