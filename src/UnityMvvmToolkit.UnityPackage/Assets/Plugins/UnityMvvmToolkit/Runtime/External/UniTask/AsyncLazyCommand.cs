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
                    ExecutionTask = _action(cancellationToken).ToAsyncLazy();
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