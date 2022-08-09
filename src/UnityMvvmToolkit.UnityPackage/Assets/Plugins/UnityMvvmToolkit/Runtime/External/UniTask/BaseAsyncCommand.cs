#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using Core;
    using System;
    using Interfaces;
    using Cysharp.Threading.Tasks;
    using System.Runtime.CompilerServices;

    public abstract class BaseAsyncCommand : BaseCommand, IBaseAsyncCommand
    {
        private AsyncLazy _executeTask;

        protected BaseAsyncCommand(Func<bool> canExecute) : base(canExecute)
        {
        }

        public bool DisableOnExecution { get; set; }

        protected AsyncLazy ExecuteTask
        {
            get => _executeTask;
            set
            {
                _executeTask = value;
                RaiseCanExecuteChanged();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool CanExecute()
        {
            if (DisableOnExecution == false)
            {
                return base.CanExecute();
            }

            return base.CanExecute() && (ExecuteTask?.Task.Status.IsCompleted() ?? true);
        }
    }
}

#endif