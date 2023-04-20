using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public partial class BindableLabel : Label, IBindableElement
    {
        private IReadOnlyProperty<string> _textProperty;
        private PropertyBindingData _propertyBindingData;

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _propertyBindingData ??= BindingTextPath.ToPropertyBindingData();

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

            objectProvider.ReturnReadOnlyProperty(_textProperty);

            _textProperty.ValueChanged -= OnPropertyValueChanged;
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
            text = newText;
        }
    }
}