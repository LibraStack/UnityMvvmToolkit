using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public partial class BindableTextField : TextField, IBindableElement
    {
        private IProperty<string> _valueProperty;
        private PropertyBindingData _propertyBindingData;

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _propertyBindingData ??= BindingValuePath.ToPropertyBindingData();

            _valueProperty = objectProvider.RentProperty<string>(context, _propertyBindingData);
            _valueProperty.ValueChanged += OnPropertyValueChanged;

            UpdateControlValue(_valueProperty.Value);
            this.RegisterValueChangedCallback(OnControlValueChanged);
        }

        public void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_valueProperty == null)
            {
                return;
            }

            try
            {
                objectProvider.ReturnProperty(_valueProperty);
                UpdateControlValue(default);
            }
            finally
            {
                _valueProperty.ValueChanged -= OnPropertyValueChanged;
                _valueProperty = null;

                this.UnregisterValueChangedCallback(OnControlValueChanged);
            }
        }

        protected virtual void OnControlValueChanged(ChangeEvent<string> e)
        {
            _valueProperty.Value = e.newValue;
        }

        private void OnPropertyValueChanged(object sender, string newValue)
        {
            UpdateControlValue(newValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void UpdateControlValue(string newValue)
        {
            SetValueWithoutNotify(newValue);
        }
    }
}