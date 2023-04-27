using UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElements
{
    public partial class BindableCheckBox : CheckBox, IBindableElement, IInitializable
    {
        private IReadOnlyProperty<string> _textProperty;
        private IProperty<bool> _isCheckedProperty;

        private PropertyBindingData _textPropertyBindingData;
        private PropertyBindingData _isCheckedPropertyBindingData;

        public void Initialize()
        {
            _textPropertyBindingData ??= BindingTextPath.ToPropertyBindingData();
            _isCheckedPropertyBindingData ??= BindingIsCheckedPath.ToPropertyBindingData();
        }

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _textProperty = objectProvider.RentReadOnlyProperty<string>(context, _textPropertyBindingData);
            _textProperty.ValueChanged += OnTextPropertyValueChanged;

            _isCheckedProperty = objectProvider.RentProperty<bool>(context, _isCheckedPropertyBindingData);
            _isCheckedProperty.ValueChanged += OnIsCheckedPropertyValueChanged;

            IsCheckedChanged += OnControlIsCheckedChanged;

            UpdateText(_textProperty.Value);
            UpdateIsCheckedState(_isCheckedProperty.Value);
        }

        public void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_textProperty == null)
            {
                return;
            }

            _textProperty.ValueChanged -= OnTextPropertyValueChanged;
            _isCheckedProperty.ValueChanged -= OnIsCheckedPropertyValueChanged;

            objectProvider.ReturnReadOnlyProperty(_textProperty);
            objectProvider.ReturnProperty(_isCheckedProperty);

            _textProperty = null;
            _isCheckedProperty = null;

            IsCheckedChanged -= OnControlIsCheckedChanged;

            UpdateText(nameof(BindableCheckBox));
            UpdateIsCheckedState(false);
        }

        private void OnControlIsCheckedChanged(object sender, bool newValue)
        {
            _isCheckedProperty.Value = newValue;
        }

        private void OnTextPropertyValueChanged(object sender, string newText)
        {
            UpdateText(newText);
        }

        private void OnIsCheckedPropertyValueChanged(object sender, bool newValue)
        {
            UpdateIsCheckedState(newValue);
        }

        private void UpdateText(string text)
        {
            Text = text;
        }

        private void UpdateIsCheckedState(bool isChecked)
        {
            SetIsCheckedWithoutNotify(isChecked);
        }
    }
}