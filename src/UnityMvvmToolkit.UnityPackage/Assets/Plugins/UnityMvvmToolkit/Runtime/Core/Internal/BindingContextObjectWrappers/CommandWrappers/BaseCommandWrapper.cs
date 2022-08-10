using System;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.CommandWrappers
{
    internal abstract class BaseCommandWrapper : IBaseCommand
    {
        private readonly IBaseCommand _baseCommand;

        protected BaseCommandWrapper(IBaseCommand command)
        {
            _baseCommand = command;
        }

        public event EventHandler<bool> CanExecuteChanged
        {
            add => _baseCommand.CanExecuteChanged += value;
            remove => _baseCommand.CanExecuteChanged -= value;
        }

        public bool CanExecute()
        {
            return _baseCommand.CanExecute();
        }

        public void RaiseCanExecuteChanged()
        {
            _baseCommand.RaiseCanExecuteChanged();
        }
    }
}