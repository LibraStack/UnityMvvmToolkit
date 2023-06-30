using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public partial class BindableButton : BaseButton, IBindableElement
    {
        private int? _buttonId;

        private IBaseCommand _command;
        private CommandBindingData _commandBindingData;

        public virtual void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            if (string.IsNullOrWhiteSpace(Command))
            {
                return;
            }

            _buttonId ??= GetHashCode();
            _commandBindingData ??= Command.ToCommandBindingData(_buttonId.Value);

            _command = string.IsNullOrEmpty(_commandBindingData.ParameterValue)
                ? objectProvider.GetCommand<ICommand>(context, _commandBindingData.PropertyName)
                : objectProvider.RentCommandWrapper(context, _commandBindingData);

            _command.CanExecuteChanged += OnCommandCanExecuteChanged;

            clicked += OnButtonClicked;
            SetControlEnabled(_command.CanExecute());
        }

        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_command is null)
            {
                return;
            }

            _command.CanExecuteChanged -= OnCommandCanExecuteChanged;

            objectProvider.ReturnCommandWrapper(_command, _commandBindingData);

            _command = null;

            clicked -= OnButtonClicked;
            SetControlEnabled(true);
        }

        private void OnButtonClicked()
        {
            _command.Execute(_buttonId!.Value);
        }

        private void OnCommandCanExecuteChanged(object sender, bool canExecute)
        {
            SetControlEnabled(canExecute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetControlEnabled(bool isEnabled)
        {
            Enabled = isEnabled;
        }
    }
}