using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.CommandWrappers
{
    internal class CommandWrapperWithoutConverter : BaseCommandWrapper, ICommandWrapperWithParameter
    {
        private readonly ICommand<ReadOnlyMemory<char>> _command;
        private readonly Dictionary<int, ReadOnlyMemory<char>> _parameters;

        public CommandWrapperWithoutConverter(ICommand<ReadOnlyMemory<char>> command) : base(command)
        {
            _command = command;
            _parameters = new Dictionary<int, ReadOnlyMemory<char>>();
        }

        public void SetParameter(int elementId, ReadOnlyMemory<char> parameter)
        {
            _parameters.Add(elementId, parameter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute(int elementId)
        {
            _command?.Execute(_parameters[elementId]);
        }
    }
}