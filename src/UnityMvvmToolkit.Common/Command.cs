using System;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common
{
    public class Command : ICommand
    {
        private readonly Action<string> _execute;

        public Command(Action<string> execute)
        {
            _execute = execute;
        }

        public void Execute(string parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}