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
            if (IsRunning && AllowConcurrency == false)
            {
                return;
            }

            ExecuteAsync().Forget();
        }

        public async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IsRunning = true;
                await _action(cancellationToken);
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}

#endif