using System;
using TMPro;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UGUI.BindableUGUIElements;

namespace UnityMvvmToolkit.UGUI.BindableUGUIElementWrappers
{
    public class BindableInputFieldWrapper : BindablePropertyElement, IInitializable, IDisposable
    {
        private readonly TMP_InputField _inputField;
        private readonly IProperty<string> _textProperty;

        public BindableInputFieldWrapper(BindableInputField inputField, IObjectProvider objectProvider) 
            : base(objectProvider)
        {
            _inputField = inputField.InputField;
            _textProperty = GetProperty<string>(inputField.BindingTextPath);
        }

        public bool CanInitialize => _textProperty != null;

        public void Initialize()
        {
            _inputField.onValueChanged.AddListener(OnInputFieldTextChanged);
        }

        public override void UpdateValues()
        {
            var value = _textProperty.Value;
            if (_inputField.text != value)
            {
                _inputField.SetTextWithoutNotify(value);
            }
        }

        public void Dispose()
        {
            _inputField.onValueChanged.RemoveListener(OnInputFieldTextChanged);
        }

        private void OnInputFieldTextChanged(string text)
        {
            _textProperty.Value = text;
        }
    }
}