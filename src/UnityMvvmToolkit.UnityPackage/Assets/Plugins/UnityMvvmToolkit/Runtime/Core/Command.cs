using System;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core
{
    public class Command : BaseCommand, ICommand
    {
        private readonly Action _action;

        public Command(Action action, Func<bool> canExecute = null) : base(canExecute)
        {
            _action = action;
        }

        public void Execute()
        {
            _action?.Invoke();
        }
    }

    public class Command<T> : BaseCommand, ICommand<T>
    {
        private readonly Action<T> _action;

        public Command(Action<T> action, Func<bool> canExecute = null) : base(canExecute)
        {
            _action = action;
        }

        public void Execute(T parameter)
        {
            _action?.Invoke(parameter);
        }
    }
}