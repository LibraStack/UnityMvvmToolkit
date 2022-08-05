using System;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;
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
            _textField = textField;
            _textField.RegisterValueChangedCallback(OnTextFieldValueChanged);

            _valueProperty = GetProperty<string>(textField.BindingValuePath);
        }

        public void Dispose()
        {
            _textField.UnregisterValueChangedCallback(OnTextFieldValueChanged);
        }

        private void OnTextFieldValueChanged(ChangeEvent<string> e)
        {
            _valueProperty.Value = e.newValue;
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
    }
}