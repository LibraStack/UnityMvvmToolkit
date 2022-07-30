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

        public BindableVisualButton(BindableButton button, IReadOnlyProperty<ICommand> property)
        {
            _button = button;
            _command = property.Value;
            _commandParameter = button.CommandParameter;

            _button.clicked += OnButtonClicked;
        }

        public void Dispose()
        {
            _button.clicked -= OnButtonClicked;
        }

        private void OnButtonClicked()
        {
            _command.Execute(_commandParameter);
        }
    }
}