using System.Runtime.CompilerServices;
using Interfaces;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace BindableUIElements
{
    public abstract class BindableBinaryStateButton : BindableButton, IBindableBinaryStateElement
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

        private void OnStateValueChanged(object sender, bool newValue)
        {
            UpdateControl(newValue);
        }

        public override void ResetBindingContext(IObjectProvider objectProvider)
        {
            base.ResetBindingContext(objectProvider);

            if (_stateProperty == null)
            {
                return;
            }

            objectProvider.ReturnReadOnlyProperty(_stateProperty);

            _stateProperty.ValueChanged -= OnStateValueChanged;
            _stateProperty = null;

            UpdateControl(false);
        }

        public abstract void Activate();
        public abstract void Deactivate();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        public new class UxmlTraits : BindableButton.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _stateAttribute = new()
                { name = "binding-state-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((BindableBinaryStateButton) visualElement).BindingStatePath =
                    _stateAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}