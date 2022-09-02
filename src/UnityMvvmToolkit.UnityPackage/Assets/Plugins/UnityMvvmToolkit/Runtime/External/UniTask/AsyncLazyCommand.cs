#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using System;
    using Interfaces;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public class AsyncLazyCommand : BaseAsyncLazyCommand, IAsyncCommand
    {
        private readonly Func<CancellationToken, UniTask> _action;

        public AsyncLazyCommand(Func<CancellationToken, UniTask> action, Func<bool> canExecute = null) 
            : base(canExecute)
        {
            _action = action;
        }

        public void Execute()
        {
            ExecuteAsync().Forget();
        }

        public async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (IsRunning == false)
                {
                    ExecutionTask = _action.Invoke(cancellationToken).ToAsyncLazy();
                }

                await ExecutionTask;
            }
            finally
            {
                ExecutionTask = null;
            }
        }
    }

    public class AsyncLazyCommand<T> : BaseAsyncLazyCommand, IAsyncCommand<T>
    {
        private readonly Func<T, CancellationToken, UniTask> _action;

        public AsyncLazyCommand(Func<T, CancellationToken, UniTask> action, Func<bool> canExecute = null) 
            : base(canExecute)
        {
            _action = action;
        }

        public void Execute(T parameter)
        {
            ExecuteAsync(parameter).Forget();
        }

        public async UniTask ExecuteAsync(T parameter, CancellationToken cancellationToken = default)
        {
            try
            {
                if (IsRunning == false)
                {
                    ExecutionTask = _action.Invoke(parameter, cancellationToken).ToAsyncLazy();
                }

                await ExecutionTask;
            }
            finally
            {
                ExecutionTask = null;
            }
        }
    }
}

#endif