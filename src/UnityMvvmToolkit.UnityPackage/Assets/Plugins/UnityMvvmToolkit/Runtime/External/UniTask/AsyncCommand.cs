#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using System;
    using Interfaces;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public class AsyncCommand : BaseAsyncCommand, IAsyncCommand
    {
        private readonly Func<CancellationToken, UniTask> _action;

        public AsyncCommand(Func<CancellationToken, UniTask> action, Func<bool> canExecute = null) : base(canExecute)
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
                IsRunning = true;
                await _action.Invoke(cancellationToken).ToAsyncLazy();
            }
            finally
            {
                IsRunning = false;
            }
        }
    }

    public class AsyncCommand<T> : BaseAsyncCommand, IAsyncCommand<T>
    {
        private readonly Func<T, CancellationToken, UniTask> _action;

        public AsyncCommand(Func<T, CancellationToken, UniTask> action, Func<bool> canExecute = null) : base(canExecute)
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
                IsRunning = true;
                await _action.Invoke(parameter, cancellationToken);
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}

#endif