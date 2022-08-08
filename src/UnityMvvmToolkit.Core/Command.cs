using System;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core
{
    public class Command : ICommand
    {
        private readonly Action _action;

        public Command(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action?.Invoke();
        }
    }

    public class Command<T> : ICommand<T>
    {
        private readonly Action<T> _action;

        public Command(Action<T> action)
        {
            _action = action;
        }

        public void Execute(T parameter)
        {
            _action?.Invoke(parameter);
        }
    }
}