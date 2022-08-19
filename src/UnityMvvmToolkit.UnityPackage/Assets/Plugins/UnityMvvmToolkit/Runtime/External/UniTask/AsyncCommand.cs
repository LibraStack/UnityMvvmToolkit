#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using System;
    using Interfaces;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public class AsyncCommand : BaseAsyncCommand, IAsyncCommand
    {
        public AsyncCommand(Func<CancellationToken, UniTask> action, Func<bool> canExecute = null) : base(canExecute)
        {
            Action = action;
        }

        protected Func<CancellationToken, UniTask> Action { get; }

        public void Execute()
        {
            ExecuteAsync().Forget();
        }

        public virtual async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                ExecuteTask = Action.Invoke(cancellationToken).ToAsyncLazy();
                await ExecuteTask;
            }
            finally
            {
                ExecuteTask = null;
            }
        }
    }

    public class AsyncCommand<T> : BaseAsyncCommand, IAsyncCommand<T>
    {
        public AsyncCommand(Func<T, CancellationToken, UniTask> action, Func<bool> canExecute = null) : base(canExecute)
        {
            Action = action;
        }

        protected Func<T, CancellationToken, UniTask> Action { get; }

        public void Execute(T parameter)
        {
            ExecuteAsync(parameter).Forget();
        }

        public virtual async UniTask ExecuteAsync(T parameter, CancellationToken cancellationToken = default)
        {
            try
            {
                ExecuteTask = Action.Invoke(parameter, cancellationToken).ToAsyncLazy();
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