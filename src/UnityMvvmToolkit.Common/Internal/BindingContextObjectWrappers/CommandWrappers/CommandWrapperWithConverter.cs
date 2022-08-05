using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.Internal.BindingContextObjectWrappers.CommandWrappers
{
    internal class CommandWrapperWithConverter<TCommandValueType> : ICommandWrapper
    {
        private readonly ReadOnlyMemory<char> _parameter;
        private readonly ICommand<TCommandValueType> _command;
        private readonly IParameterConverter<TCommandValueType> _parameterConverter;

        public CommandWrapperWithConverter(ICommand<TCommandValueType> command, ReadOnlyMemory<char> parameter,
            IParameterConverter<TCommandValueType> parameterConverter)
        {
            _command = command;
            _parameter = parameter;
            _parameterConverter = parameterConverter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute()
        {
            if (_command == null)
            {
                return;
            }

            if (_parameterConverter.TryConvert(_parameter, out var parameter))
            {
                _command.Execute(parameter);
            }
        }
    }
}