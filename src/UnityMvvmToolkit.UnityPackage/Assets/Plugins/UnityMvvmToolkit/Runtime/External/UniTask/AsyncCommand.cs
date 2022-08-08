#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using System;
    using Interfaces;
    using Cysharp.Threading.Tasks;

    public class AsyncCommand : IAsyncCommand
    {
        private AsyncLazy _executeTask;
        private readonly Func<UniTask> _action;

        public AsyncCommand(Func<UniTask> action)
        {
            _action = action;
        }

        public void Execute()
        {
            ExecuteAsync().Forget();
        }

        private async UniTask ExecuteAsync()
        {
            if (_executeTask?.Task.Status.IsCompleted() ?? true)
            {
                _executeTask = _action.Invoke().ToAsyncLazy();
            }

            await _executeTask;
        }
    }

    public class AsyncCommand<T> : IAsyncCommand<T>
    {
        private AsyncLazy _executeTask;
        private readonly Func<T, UniTask> _action;

        public AsyncCommand(Func<T, UniTask> action)
        {
            _action = action;
        }

        public void Execute(T parameter)
        {
            ExecuteAsync(parameter).Forget();
        }

        private async UniTask ExecuteAsync(T parameter)
        {
            if (_executeTask?.Task.Status.IsCompleted() ?? true)
            {
                _executeTask = _action.Invoke(parameter).ToAsyncLazy();
            }

            await _executeTask;
        }
    }
}

#endif