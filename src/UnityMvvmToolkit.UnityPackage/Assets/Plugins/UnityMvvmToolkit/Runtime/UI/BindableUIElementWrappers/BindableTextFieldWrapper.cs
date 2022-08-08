using System;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace UnityMvvmToolkit.UI.BindableUIElementWrappers
{
    // TODO: Reset value on leave.
    public class BindableTextFieldWrapper : BindablePropertyElement, IDisposable
    {
        private readonly BindableTextField _textField;
        private readonly IProperty<string> _valueProperty;

        public BindableTextFieldWrapper(BindableTextField textField, IObjectProvider objectProvider)
            : base(objectProvider)
        {
            _valueProperty = GetProperty<string>(textField.BindingValuePath);

            if (_valueProperty == null)
            {
                return;
            }

            _textField = textField;
            _textField.RegisterValueChangedCallback(OnTextFieldValueChanged);
        }

        public override void UpdateValues()
        {
            if (_valueProperty == null)
            {
                return;
            }

            var value = _valueProperty.Value;
            if (_textField.value != value)
            {
                _textField.SetValueWithoutNotify(value);
            }
        }

        public void Dispose()
        {
            _textField?.UnregisterValueChangedCallback(OnTextFieldValueChanged);
        }

        private void OnTextFieldValueChanged(ChangeEvent<string> e)
        {
            _valueProperty.Value = e.newValue;
        }
    }
}