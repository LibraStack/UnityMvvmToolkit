using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core
{
    public abstract class BaseCommand : IBaseCommand
    {
        private readonly Func<bool> _canExecute;
        private bool? _previousCanExecuteState;

        protected BaseCommand(Func<bool> canExecute)
        {
            _canExecute = canExecute;
        }

        public virtual event EventHandler<bool> CanExecuteChanged;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RaiseCanExecuteChanged()
        {
            var canExecute = CanExecute();
            if (_previousCanExecuteState == canExecute)
            {
                return;
            }

            _previousCanExecuteState = canExecute;
            CanExecuteChanged?.Invoke(this, canExecute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool CanExecute()
        {
            return _canExecute == null || _canExecute();
        }
    }
}