#if UNITYMVVMTOOLKIT_TEXTMESHPRO_SUPPORT

using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UGUI.BindableUGUIElements
{
    [RequireComponent(typeof(Button))]
    public class BindableButton : MonoBehaviour, IBindableElement
    {
        [SerializeField] private Button _button;
        [SerializeField] private string _commandPath;

        private int? _buttonId;

        private IBaseCommand _command;
        private CommandBindingData _commandBindingData;

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _buttonId ??= GetHashCode();
            _commandBindingData ??= _commandPath.ToCommandBindingData(_buttonId.Value);

            _command = string.IsNullOrEmpty(_commandBindingData.ParameterValue)
                ? objectProvider.GetCommand<ICommand>(context, _commandBindingData.PropertyName)
                : objectProvider.RentCommandWrapper(context, _commandBindingData);

            _command.CanExecuteChanged += OnCommandCanExecuteChanged;

            _button.onClick.AddListener(OnButtonClicked);
            SetControlEnabled(_command.CanExecute());
        }

        public void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_command == null)
            {
                return;
            }

            _command.CanExecuteChanged -= OnCommandCanExecuteChanged;

            objectProvider.ReturnCommandWrapper(_command, _commandBindingData);

            _command = null;

            _button.onClick.RemoveListener(OnButtonClicked);
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
            _button.enabled = isEnabled;
        }
    }
}

#endif