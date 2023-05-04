#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using Core;
    using System;
    using Interfaces;
    using Extensions;
    using System.Runtime.CompilerServices;

    public abstract class BaseAsyncCommand : BaseCommand, IBaseAsyncCommand
    {
        private bool _isRunning;

        protected BaseAsyncCommand(Func<bool> canExecute) : base(canExecute)
        {
        }

        public virtual bool IsRunning
        {
            get => _isRunning;
            protected set
            {
                _isRunning = value;
                RaiseCanExecuteChanged();
            }
        }

        public bool AllowConcurrency { get; set; }

        public virtual bool DisableOnExecution { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool CanExecute()
        {
            if (DisableOnExecution == false)
            {
                return base.CanExecute();
            }

            return IsRunning == false && base.CanExecute();
        }

        public virtual void Cancel()
        {
            throw new InvalidOperationException(
                $"To make the 'AsyncCommand' cancelable, use '{nameof(AsyncCommandExtensions.WithCancellation)}' extension.");
        }
    }
}

#endif