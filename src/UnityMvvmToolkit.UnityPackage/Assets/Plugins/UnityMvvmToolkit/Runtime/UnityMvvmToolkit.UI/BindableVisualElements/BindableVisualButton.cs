using System;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace UnityMvvmToolkit.UI.BindableVisualElements
{
    public class BindableVisualButton : IBindableElement, IDisposable
    {
        private readonly BindableButton _button;
        private readonly ICommand _command;
        private readonly string _commandParameter;

        public BindableVisualButton(BindableButton button, IPropertyProvider propertyProvider)
        {
            _button = button;
            _command = propertyProvider.GetReadOnlyProperty<ICommand>(_button.Command)?.Value;
            _commandParameter = button.CommandParameter;

            if (_command != null)
            {
                _button.clicked += OnButtonClicked;
            }
        }

        public void Dispose()
        {
            if (_command != null)
            {
                _button.clicked -= OnButtonClicked;
            }
        }

        private void OnButtonClicked()
        {
            _command.Execute(_commandParameter);
        }
    }
}