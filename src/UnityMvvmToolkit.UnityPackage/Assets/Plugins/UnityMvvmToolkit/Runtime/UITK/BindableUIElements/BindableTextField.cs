using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public partial class BindableTextField : TextField, IBindableElement
    {
        private PropertyBindingData _propertyBindingData;

        private IProperty<string> _valueProperty;
        private IReadOnlyProperty<string> _valueReadOnlyProperty;

        public virtual void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            if (string.IsNullOrWhiteSpace(BindingValuePath))
            {
                return;
            }

            _propertyBindingData ??= BindingValuePath.ToPropertyBindingData();

            if (objectProvider.TryRentProperty(context, _propertyBindingData, out _valueProperty))
            {
                _valueReadOnlyProperty = _valueProperty;
                this.RegisterValueChangedCallback(OnControlValueChanged);
            }
            else if (isReadOnly)
            {
                _valueReadOnlyProperty = objectProvider.RentReadOnlyProperty<string>(context, _propertyBindingData);
            }
            else
            {
                var controlName = string.IsNullOrWhiteSpace(name) ? nameof(BindableTextField) : name;

                throw new InvalidOperationException(
                    $"The {_propertyBindingData.PropertyName} property is read-only. Mark the {controlName} as read-only or change the property type.");
            }

            _valueReadOnlyProperty.ValueChanged += OnPropertyValueChanged;

            UpdateControlValue(_valueReadOnlyProperty.Value);
        }

        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_valueReadOnlyProperty is null)
            {
                return;
            }

            _valueReadOnlyProperty.ValueChanged -= OnPropertyValueChanged;

            if (_valueProperty is null)
            {
                objectProvider.ReturnReadOnlyProperty(_valueReadOnlyProperty);
            }
            else
            {
                objectProvider.ReturnProperty(_valueProperty);
                this.UnregisterValueChangedCallback(OnControlValueChanged);
            }

            _valueProperty = null;
            _valueReadOnlyProperty = null;

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