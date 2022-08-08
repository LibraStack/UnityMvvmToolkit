using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.CommandWrappers
{
    internal class CommandWrapper : ICommandWrapper
    {
        private readonly ICommand _command;

        public CommandWrapper(ICommand command)
        {
            _command = command;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute()
        {
            _command?.Execute();
        }
    }

    internal class CommandWrapper<T> : ICommandWrapper
    {
        private readonly T _parameter;
        private readonly ICommand<T> _command;

        public CommandWrapper(ICommand<T> command, T parameter)
        {
            _command = command;
            _parameter = parameter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute()
        {
            _command?.Execute(_parameter);
        }
    }
}