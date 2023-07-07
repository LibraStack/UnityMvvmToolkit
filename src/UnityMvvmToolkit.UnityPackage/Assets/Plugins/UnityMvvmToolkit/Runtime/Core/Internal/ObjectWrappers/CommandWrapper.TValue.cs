using System;
using System.Collections.Generic;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.ObjectWrappers
{
    internal sealed class CommandWrapper<TValue> : ICommandWrapper
    {
        private readonly Dictionary<int, TValue> _parameters;
        private readonly IParameterValueConverter<TValue> _parameterConverter;

        private int _commandId;
        private int _converterId;

        private ICommand<TValue> _command;

        [Preserve]
        public CommandWrapper(IParameterValueConverter<TValue> parameterConverter)
        {
            _commandId = -1;
            _converterId = -1;

            _parameters = new Dictionary<int, TValue>();
            _parameterConverter = parameterConverter;
        }

        public int CommandId => _commandId;
        public int ConverterId => _converterId;

        public event EventHandler<bool> CanExecuteChanged;

        public ICommandWrapper SetConverterId(int converterId)
        {
            if (_converterId != -1)
            {
                throw new InvalidOperationException("Can not change converter ID.");
            }

            _converterId = converterId;

            return this;
        }

        public ICommandWrapper SetCommand(int commandId, IBaseCommand command)
        {
            if (_command is not null)
            {
                throw new InvalidOperationException(
                    $"{nameof(CommandWrapper<TValue>)} was not reset.");
            }

            _commandId = commandId;

            _command = (ICommand<TValue>) command;
            _command.CanExecuteChanged += OnCommandCanExecuteChanged;

            return this;
        }

        public ICommandWrapper RegisterParameter(int elementId, string parameter)
        {
            _parameters.Add(elementId, _parameterConverter.Convert(parameter));

            return this;
        }

        public int UnregisterParameter(int elementId)
        {
            _parameters.Remove(elementId);

            return _parameters.Count;
        }

        public bool CanExecute()
        {
            return _command.CanExecute();
        }

        public void RaiseCanExecuteChanged()
        {
            _command.RaiseCanExecuteChanged();
        }

        void IBaseCommand.Execute(int elementId)
        {
            _command.Execute(_parameters[elementId]);
        }

        public void Reset()
        {
            _command.CanExecuteChanged -= OnCommandCanExecuteChanged;
            _command = null;

            _commandId = -1;
        }

        private void OnCommandCanExecuteChanged(object sender, bool canExecute)
        {
            CanExecuteChanged?.Invoke(this, canExecute);
        }
    }
}