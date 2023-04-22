using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace BindableUIElements
{
    public partial class BindableStrikethroughLabel : BindableLabel
    {
        private const string LabelDoneClassName = "task-item__label--done";

        private IReadOnlyProperty<bool> _isDoneProperty;
        private PropertyBindingData _propertyBindingData;

        public override void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            base.SetBindingContext(context, objectProvider);

            _propertyBindingData ??= BindingIsDonePath.ToPropertyBindingData();

            _isDoneProperty = objectProvider.RentReadOnlyProperty<bool>(context, _propertyBindingData);
            _isDoneProperty.ValueChanged += OnPropertyValueChanged;

            UpdateControlState(_isDoneProperty.Value);
        }

        public override void ResetBindingContext(IObjectProvider objectProvider)
        {
            base.ResetBindingContext(objectProvider);

            if (_isDoneProperty == null)
            {
                return;
            }

            objectProvider.ReturnReadOnlyProperty(_isDoneProperty);

            _isDoneProperty.ValueChanged -= OnPropertyValueChanged;
            _isDoneProperty = null;

            UpdateControlState(false);
        }

        private void OnPropertyValueChanged(object sender, bool isDone)
        {
            UpdateControlState(isDone);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateControlState(bool isDone)
        {
            if (isDone)
            {
                AddToClassList(LabelDoneClassName);
            }
            else
            {
                RemoveFromClassList(LabelDoneClassName);
            }
        }
    }
}