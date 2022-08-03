using System;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace UnityMvvmToolkit.UI.BindableVisualElements
{
    // TODO: Reset value on leave.
    public class BindableVisualTextField : BindableVisualElement, IDisposable
    {
        private readonly BindableTextField _textField;
        private readonly IProperty<string> _textProperty;

        public BindableVisualTextField(BindableTextField textField, IPropertyProvider propertyProvider) 
            : base(propertyProvider)
        {
            _textField = textField;
            _textField.RegisterValueChangedCallback(OnTextFieldValueChanged);

            _textProperty = GetProperty<string>(textField.BindingValuePath);
        }

        public void Dispose()
        {
            _textField.UnregisterValueChangedCallback(OnTextFieldValueChanged);
        }

        private void OnTextFieldValueChanged(ChangeEvent<string> e)
        {
            _textProperty.Value = e.newValue;
        }

        public override void UpdateValues()
        {
            if (_textProperty == null)
            {
                return;
            }

            var textPropertyValue = _textProperty.Value;

            if (_textField.value != textPropertyValue)
            {
                _textField.SetValueWithoutNotify(_textProperty.Value);
            }
        }
    }
}