#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.Internal
{
    using System;
    using Interfaces;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;

    internal class AsyncCommandWithCancellation : BaseAsyncCommand, IAsyncCommand
    {
        private readonly IAsyncCommand _asyncCommand;
        private readonly ConcurrentQueue<UniTask> _runningCommands;

        private CancellationTokenSource _cancellationTokenSource;

        public AsyncCommandWithCancellation(IAsyncCommand asyncCommand) : base(null)
        {
            _asyncCommand = asyncCommand;
            _runningCommands = new ConcurrentQueue<UniTask>();
        }

        public override bool DisableOnExecution
        {
            get => _asyncCommand.DisableOnExecution;
            set => _asyncCommand.DisableOnExecution = value;
        }

        public override event EventHandler<bool> CanExecuteChanged
        {
            add => _asyncCommand.CanExecuteChanged += value;
            remove => _asyncCommand.CanExecuteChanged -= value;
        }

        public void Execute()
        {
            if (IsCommandRunning)
            {
                TryEnqueueAsyncCommand(_cancellationTokenSource.Token);
            }
            else
            {
                ExecuteAsync().Forget();
            }
        }

        public async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource ??= new CancellationTokenSource();

            try
            {
                SetCommandRunning(true);

                TryEnqueueAsyncCommand(_cancellationTokenSource.Token);

                while (_runningCommands.TryDequeue(out var asyncCommand))
                {
                    await asyncCommand.SuppressCancellationThrow();
                }
            }
            finally
            {
                SetCommandRunning(false);

                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public override void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TryEnqueueAsyncCommand(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            _runningCommands.Enqueue(_asyncCommand.ExecuteAsync(cancellationToken));
        }
    }
}

#endif