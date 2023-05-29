#if UNITYMVVMTOOLKIT_TEXTMESHPRO_SUPPORT

using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UGUI.BindableUGUIElements
{
    public class BindableInputField : MonoBehaviour, IBindableElement
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private string _bindingTextPath;

        private IProperty<string> _valueProperty;
        private PropertyBindingData _propertyBindingData;

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _propertyBindingData ??= _bindingTextPath.ToPropertyBindingData();

            _valueProperty = objectProvider.RentProperty<string>(context, _propertyBindingData);
            _valueProperty.ValueChanged += OnPropertyValueChanged;

            UpdateControlValue(_valueProperty.Value);
            _inputField.onValueChanged.AddListener(OnControlValueChanged);
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

            _inputField.onValueChanged.RemoveListener(OnControlValueChanged);
            UpdateControlValue(default);
        }

        protected virtual void OnControlValueChanged(string newValue)
        {
            _valueProperty.Value = newValue;
        }

        private void OnPropertyValueChanged(object sender, string newValue)
        {
            UpdateControlValue(newValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void UpdateControlValue(string newValue)
        {
            _inputField.SetTextWithoutNotify(newValue);
        }
    }
}

#endif