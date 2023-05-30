#if UNITYMVVMTOOLKIT_TEXTMESHPRO_SUPPORT

using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UGUI.BindableUGUIElements
{
    [RequireComponent(typeof(TMP_Text))]
    public class BindableLabel : MonoBehaviour, IBindableElement
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private string _bindingTextPath;

        private IReadOnlyProperty<string> _textProperty;
        private PropertyBindingData _propertyBindingData;

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _propertyBindingData ??= _bindingTextPath.ToPropertyBindingData();

            _textProperty = objectProvider.RentReadOnlyProperty<string>(context, _propertyBindingData);
            _textProperty.ValueChanged += OnPropertyValueChanged;

            UpdateControlText(_textProperty.Value);
        }

        public void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_textProperty == null)
            {
                return;
            }

            _textProperty.ValueChanged -= OnPropertyValueChanged;

            objectProvider.ReturnReadOnlyProperty(_textProperty);

            _textProperty = null;

            UpdateControlText(default);
        }

        private void OnPropertyValueChanged(object sender, string newText)
        {
            UpdateControlText(newText);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void UpdateControlText(string newText)
        {
            _label.text = newText;
        }
    }
}

#endif