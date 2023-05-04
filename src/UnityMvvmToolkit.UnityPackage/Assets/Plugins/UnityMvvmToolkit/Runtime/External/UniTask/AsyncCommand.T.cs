#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using System;
    using Interfaces;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public class AsyncCommand<T> : BaseAsyncCommand, IAsyncCommand<T>
    {
        private readonly Func<T, CancellationToken, UniTask> _action;

        public AsyncCommand(Func<T, CancellationToken, UniTask> action, Func<bool> canExecute = null) : base(canExecute)
        {
            _action = action;
        }

        public void Execute(T parameter)
        {
            if (IsCommandRunning && AllowConcurrency == false)
            {
                return;
            }

            ExecuteAsync(parameter).Forget();
        }

        public async UniTask ExecuteAsync(T parameter, CancellationToken cancellationToken = default)
        {
            try
            {
                SetCommandRunning(true);

                await _action(parameter, cancellationToken);
            }
            finally
            {
                SetCommandRunning(false);
            }
        }
    }
}

#endif