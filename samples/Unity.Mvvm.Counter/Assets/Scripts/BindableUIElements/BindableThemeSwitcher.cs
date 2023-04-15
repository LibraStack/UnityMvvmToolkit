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
        }

        public void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_valueProperty == null)
            {
                return;
            }

            objectProvider.ReturnProperty(_valueProperty);

            _valueProperty.ValueChanged -= OnPropertyValueChanged;
            _valueProperty = null;
        }

        protected override void OnControlValueChanged(bool value)
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