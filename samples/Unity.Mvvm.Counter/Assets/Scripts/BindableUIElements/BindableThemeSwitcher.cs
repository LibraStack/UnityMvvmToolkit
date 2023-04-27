using UIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElements
{
    public partial class BindableThemeSwitcher : ThemeSwitcher, IBindableElement
    {
        private IProperty<bool> _valueProperty;
        private PropertyBindingData _propertyBindingData;

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _propertyBindingData ??= BindingValuePath.ToPropertyBindingData();

            _valueProperty = objectProvider.RentProperty<bool>(context, _propertyBindingData);
            _valueProperty.ValueChanged += OnPropertyValueChanged;

            UpdateControlValue(_valueProperty.Value);

            ValueChanged += OnControlValueChanged;
        }

        public void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_valueProperty == null)
            {
                return;
            }

            _valueProperty.ValueChanged -= OnPropertyValueChanged;

            objectProvider.ReturnProperty(_valueProperty);

            _valueProperty = null;

            ValueChanged -= OnControlValueChanged;

            UpdateControlValue(false);
        }

        private void OnControlValueChanged(object sender, bool value)
        {
            _valueProperty.Value = value;
        }

        private void OnPropertyValueChanged(object sender, bool newValue)
        {
            UpdateControlValue(newValue);
        }

        private void UpdateControlValue(bool newValue)
        {
            SetValueWithoutNotify(newValue);
        }
    }
}