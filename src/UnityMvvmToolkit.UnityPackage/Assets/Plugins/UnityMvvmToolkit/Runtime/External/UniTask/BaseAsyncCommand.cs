#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using Core;
    using System;
    using Interfaces;
    using Extensions;
    using System.Runtime.CompilerServices;
    using UnityMvvmToolkit.Core.Interfaces;

    public abstract class BaseAsyncCommand : BaseCommand, IBaseAsyncCommand
    {
        private readonly IProperty<bool> _isRunning;

        protected BaseAsyncCommand(Func<bool> canExecute) : base(canExecute)
        {
            _isRunning = new Property<bool>();
        }

        public bool AllowConcurrency { get; set; }
        public virtual bool DisableOnExecution { get; set; }

        public IReadOnlyProperty<bool> IsRunning => _isRunning;

        protected bool IsCommandRunning { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool CanExecute()
        {
            if (DisableOnExecution == false)
            {
                return base.CanExecute();
            }

            return IsCommandRunning == false && base.CanExecute();
        }

        public virtual void Cancel()
        {
            throw new InvalidOperationException(
                $"To make the 'AsyncCommand' cancelable, use '{nameof(AsyncCommandExtensions.WithCancellation)}' extension.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SetCommandRunning(bool isRunning)
        {
            _isRunning.Value = isRunning;

            if (IsCommandRunning == isRunning)
            {
                return;
            }

            IsCommandRunning = isRunning;
            RaiseCanExecuteChanged();
        }
    }
}

#endif