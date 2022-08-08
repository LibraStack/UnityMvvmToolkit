#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using System;
    using Interfaces;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public class AsyncCommand : IAsyncCommand
    {
        private AsyncLazy _executeTask;
        private readonly Func<CancellationToken, UniTask> _action;

        public AsyncCommand(Func<CancellationToken, UniTask> action)
        {
            _action = action;
        }

        public void Execute()
        {
            ExecuteAsync().Forget();
        }

        public async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (_executeTask?.Task.Status.IsCompleted() ?? true)
            {
                _executeTask = _action.Invoke(cancellationToken).ToAsyncLazy();
            }

            await _executeTask;
        }
    }

    public class AsyncCommand<T> : IAsyncCommand<T>
    {
        private AsyncLazy _executeTask;
        private readonly Func<T, CancellationToken, UniTask> _action;

        public AsyncCommand(Func<T, CancellationToken, UniTask> action)
        {
            _action = action;
        }

        public void Execute(T parameter)
        {
            ExecuteAsync(parameter).Forget();
        }

        public async UniTask ExecuteAsync(T parameter, CancellationToken cancellationToken = default)
        {
            if (_executeTask?.Task.Status.IsCompleted() ?? true)
            {
                _executeTask = _action.Invoke(parameter, cancellationToken).ToAsyncLazy();
            }

            await _executeTask;
        }
    }
}

#endif