using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace UnityMvvmToolkit.UI.BindableVisualElements
{
    // TODO: Reset value on leave.
    public class BindableVisualTextField<TValueType> : TwoWayBindableElement<TValueType>, IDisposable
    {
        private readonly BindableTextField _textField;
        private readonly IValueConverter<TValueType, string> _valueConverter;

        public BindableVisualTextField(BindableTextField textField, IProperty<TValueType> property,
            IValueConverter<TValueType, string> valueConverter) : base(property)
        {
            _valueConverter = valueConverter;

            _textField = textField;
            _textField.RegisterValueChangedCallback(OnTextFieldValueChanged);
        }

        public void Dispose()
        {
            _textField.UnregisterValueChangedCallback(OnTextFieldValueChanged);
        }

        private void OnTextFieldValueChanged(ChangeEvent<string> e)
        {
            if (_valueConverter.TryConvertBack(e.newValue, out var newValue))
            {
                Property.Value = newValue;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool TryGetElementValue(out TValueType value)
        {
            return _valueConverter.TryConvertBack(_textField.value, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void OnPropertyValueChanged(TValueType newValue)
        {
            if (_valueConverter.TryConvert(newValue, out var result))
            {
                _textField.SetValueWithoutNotify(result);
            }
        }
    }
}