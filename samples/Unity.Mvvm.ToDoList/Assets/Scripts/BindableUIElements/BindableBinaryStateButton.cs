using Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace BindableUIElements
{
    public abstract partial class BindableBinaryStateButton : BindableButton, IBindableBinaryStateElement
    {
        private IReadOnlyProperty<bool> _stateProperty;
        private PropertyBindingData _statePathBindingData;

        public string BindingStatePath { get; private set; }

        public override void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            base.SetBindingContext(context, objectProvider);

            _statePathBindingData ??= BindingStatePath.ToPropertyBindingData();

            _stateProperty = objectProvider.RentReadOnlyProperty<bool>(context, _statePathBindingData);
            _stateProperty.ValueChanged += OnStateValueChanged;

            UpdateControl(_stateProperty.Value);
        }

        public override void ResetBindingContext(IObjectProvider objectProvider)
        {
            base.ResetBindingContext(objectProvider);

            if (_stateProperty == null)
            {
                return;
            }

            _stateProperty.ValueChanged -= OnStateValueChanged;

            objectProvider.ReturnReadOnlyProperty(_stateProperty);

            _stateProperty = null;

            UpdateControl(false);
        }

        public abstract void Activate();
        public abstract void Deactivate();

        private void OnStateValueChanged(object sender, bool newValue)
        {
            UpdateControl(newValue);
        }

        private void UpdateControl(bool value)
        {
            if (value)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }
    }
}