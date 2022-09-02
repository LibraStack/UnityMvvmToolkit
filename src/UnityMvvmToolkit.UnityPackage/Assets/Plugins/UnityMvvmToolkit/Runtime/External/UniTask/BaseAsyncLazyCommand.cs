#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using System;
    using Cysharp.Threading.Tasks;

    public abstract class BaseAsyncLazyCommand : BaseAsyncCommand
    {
        private AsyncLazy _executionTask;

        protected BaseAsyncLazyCommand(Func<bool> canExecute) : base(canExecute)
        {
        }

        public override bool IsRunning => ExecutionTask is { Task: { Status: UniTaskStatus.Pending } };

        protected AsyncLazy ExecutionTask
        {
            get => _executionTask;
            set
            {
                _executionTask = value;
                RaiseCanExecuteChanged();
            }
        }
    }
}

#endif