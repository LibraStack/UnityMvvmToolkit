using System;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common
{
    public class Command : ICommand
    {
        private readonly Action _execute;

        public Command(Action execute)
        {
            _execute = execute;
        }

        public void Execute()
        {
            _execute?.Invoke();
        }
    }

    public class Command<T> : ICommand<T>
    {
        private readonly Action<T> _execute;

        public Command(Action<T> execute)
        {
            _execute = execute;
        }

        public void Execute(T parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}