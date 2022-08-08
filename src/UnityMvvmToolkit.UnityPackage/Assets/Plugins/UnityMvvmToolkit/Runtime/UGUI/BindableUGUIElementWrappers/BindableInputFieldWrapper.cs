using System;
using TMPro;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UGUI.BindableUGUIElements;

namespace UnityMvvmToolkit.UGUI.BindableUGUIElementWrappers
{
    public class BindableInputFieldWrapper : BindablePropertyElement, IDisposable
    {
        private readonly TMP_InputField _inputField;
        private readonly IProperty<string> _textProperty;

        public BindableInputFieldWrapper(BindableInputField inputField, IObjectProvider objectProvider) : base(
            objectProvider)
        {
            _textProperty = GetProperty<string>(inputField.BindingTextPath);

            if (_textProperty == null)
            {
                return;
            }

            _inputField = inputField.InputField;
            _inputField.onValueChanged.AddListener(OnInputFieldTextChanged);
        }

        public override void UpdateValues()
        {
            if (_textProperty == null)
            {
                return;
            }

            var value = _textProperty.Value;
            if (_inputField.text != value)
            {
                _inputField.SetTextWithoutNotify(value);
            }
        }

        public void Dispose()
        {
            if (_inputField != null)
            {
                _inputField.onValueChanged.RemoveListener(OnInputFieldTextChanged);
            }
        }

        private void OnInputFieldTextChanged(string text)
        {
            _textProperty.Value = text;
        }
    }
}