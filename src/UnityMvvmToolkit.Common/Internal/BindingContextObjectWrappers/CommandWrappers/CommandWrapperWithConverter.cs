using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.Internal.BindingContextObjectWrappers.CommandWrappers
{
    internal class CommandWrapperWithConverter<TCommandValueType> : ICommandWrapper
    {
        private readonly TCommandValueType _parameter;
        private readonly ICommand<TCommandValueType> _command;

        public CommandWrapperWithConverter(ICommand<TCommandValueType> command, ReadOnlyMemory<char> parameter,
            IParameterConverter<TCommandValueType> parameterConverter)
        {
            _command = command;
            _parameter = parameterConverter.Convert(parameter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute()
        {
            _command?.Execute(_parameter);
        }
    }
}