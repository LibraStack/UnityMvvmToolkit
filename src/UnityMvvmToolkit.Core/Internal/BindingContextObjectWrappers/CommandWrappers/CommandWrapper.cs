using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.CommandWrappers
{
    internal class CommandWrapper : BaseCommandWrapper, ICommandWrapperWithParameter
    {
        private readonly ICommand<string> _command;
        private readonly Dictionary<int, string> _parameters;

        public CommandWrapper(ICommand<string> command) : base(command)
        {
            _command = command;
            _parameters = new Dictionary<int, string>();
        }

        public ICommandWrapper RegisterParameter(int elementId, string parameter)
        {
            _parameters.Add(elementId, parameter);

            return this;
        }

        public ICommandWrapper UnregisterParameter(int elementId)
        {
            _parameters.Remove(elementId);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute(int elementId)
        {
            _command?.Execute(_parameters[elementId]);
        }
    }
}