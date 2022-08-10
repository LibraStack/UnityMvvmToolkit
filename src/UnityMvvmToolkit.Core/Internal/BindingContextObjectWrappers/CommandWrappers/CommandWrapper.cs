using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.CommandWrappers
{
    internal class CommandWrapper : BaseCommandWrapper, ICommandWrapper
    {
        private readonly ICommand _command;

        public CommandWrapper(ICommand command) : base(command)
        {
            _command = command;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute(int elementId)
        {
            _command?.Execute();
        }
    }
}