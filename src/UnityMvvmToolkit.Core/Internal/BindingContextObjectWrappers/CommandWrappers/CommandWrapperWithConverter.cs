using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.CommandWrappers
{
    internal class CommandWrapperWithConverter<TCommandValueType> : BaseCommandWrapper, ICommandWrapperWithParameter
    {
        private readonly ICommand<TCommandValueType> _command;
        private readonly IParameterValueConverter<TCommandValueType> _parameterConverter;
        private readonly Dictionary<int, TCommandValueType> _parameters;

        public CommandWrapperWithConverter(ICommand<TCommandValueType> command,
            IParameterValueConverter<TCommandValueType> parameterConverter) : base(command)
        {
            _command = command;
            _parameterConverter = parameterConverter;
            _parameters = new Dictionary<int, TCommandValueType>();
        }

        public void SetParameter(int elementId, string parameter)
        {
            _parameters.Add(elementId, _parameterConverter.Convert(parameter));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute(int elementId)
        {
            _command?.Execute(_parameters[elementId]);
        }
    }
}