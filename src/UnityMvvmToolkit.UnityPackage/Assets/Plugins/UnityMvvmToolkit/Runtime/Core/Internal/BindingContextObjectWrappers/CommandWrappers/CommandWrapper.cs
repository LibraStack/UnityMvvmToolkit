using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.CommandWrappers
{
    internal class CommandWrapper : BaseCommandWrapper
    {
        private readonly ICommand _command;

        public CommandWrapper(ICommand command) : base(command)
        {
            _command = command;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Execute()
        {
            _command?.Execute();
        }
    }

    internal class CommandWrapper<T> : BaseCommandWrapper
    {
        private readonly T _parameter;
        private readonly ICommand<T> _command;

        public CommandWrapper(ICommand<T> command, T parameter) : base(command)
        {
            _command = command;
            _parameter = parameter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Execute()
        {
            _command?.Execute(_parameter);
        }
    }
}