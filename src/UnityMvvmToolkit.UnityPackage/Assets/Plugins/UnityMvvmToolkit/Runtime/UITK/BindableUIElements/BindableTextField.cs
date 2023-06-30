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

        public virtual void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            if (string.IsNullOrWhiteSpace(BindingValuePath))
            {
                return;
            }

            _propertyBindingData ??= BindingValuePath.ToPropertyBindingData();

            _valueProperty = objectProvider.RentProperty<string>(context, _propertyBindingData);
            _valueProperty.ValueChanged += OnPropertyValueChanged;

            UpdateControlValue(_valueProperty.Value);
            this.RegisterValueChangedCallback(OnControlValueChanged);
        }

        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_valueProperty is null)
            {
                return;
            }

            _valueProperty.ValueChanged -= OnPropertyValueChanged;

            objectProvider.ReturnProperty(_valueProperty);

            _valueProperty = null;

            this.UnregisterValueChangedCallback(OnControlValueChanged);
            UpdateControlValue(default);
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