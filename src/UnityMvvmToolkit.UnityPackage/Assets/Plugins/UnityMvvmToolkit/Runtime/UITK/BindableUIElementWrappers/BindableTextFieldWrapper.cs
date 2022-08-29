using System;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace UnityMvvmToolkit.UITK.BindableUIElementWrappers
{
    // TODO: Reset value on leave.
    public class BindableTextFieldWrapper : BindablePropertyElement, IInitializable, IDisposable
    {
        private readonly BindableTextField _textField;
        private readonly IProperty<string> _valueProperty;

        public BindableTextFieldWrapper(BindableTextField textField, IObjectProvider objectProvider)
            : base(objectProvider)
        {
            _textField = textField;
            _valueProperty = GetProperty<string>(textField.BindingValuePath);
        }

        public bool CanInitialize => _valueProperty != null;

        public void Initialize()
        {
            _textField.RegisterValueChangedCallback(OnTextFieldValueChanged);
        }

        public override void UpdateValues()
        {
            var value = _valueProperty.Value;
            if (_textField.value != value)
            {
                _textField.SetValueWithoutNotify(value);
            }
        }

        public void Dispose()
        {
            _textField.UnregisterValueChangedCallback(OnTextFieldValueChanged);
        }

        private void OnTextFieldValueChanged(ChangeEvent<string> e)
        {
            _valueProperty.Value = e.newValue;
        }
    }
}