#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.Internal
{
    using System;
    using Interfaces;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    internal class AsyncCommandWithCancellation : BaseAsyncCommand, IAsyncCommand
    {
        private readonly IAsyncCommand _asyncCommand;
        private CancellationTokenSource _cancellationTokenSource;

        public AsyncCommandWithCancellation(IAsyncCommand asyncCommand) : base(null)
        {
            _asyncCommand = asyncCommand;
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
            ExecuteAsync().Forget();
        }

        public async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await _asyncCommand.ExecuteAsync(_cancellationTokenSource.Token);
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public override void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }
    }

    internal class AsyncCommandWithCancellation<T> : BaseAsyncCommand, IAsyncCommand<T>
    {
        private readonly IAsyncCommand<T> _asyncCommand;
        private CancellationTokenSource _cancellationTokenSource;

        public AsyncCommandWithCancellation(IAsyncCommand<T> asyncCommand) : base(null)
        {
            _asyncCommand = asyncCommand;
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

        public void Execute(T parameter)
        {
            ExecuteAsync(parameter).Forget();
        }

        public async UniTask ExecuteAsync(T parameter, CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await _asyncCommand.ExecuteAsync(parameter, _cancellationTokenSource.Token);
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public override void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}

#endif