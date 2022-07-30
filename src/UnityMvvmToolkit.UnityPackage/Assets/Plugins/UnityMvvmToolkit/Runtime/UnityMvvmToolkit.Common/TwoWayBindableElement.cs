using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common
{
    public abstract class TwoWayBindableElement<TValueType> : IBindableVisualElement
    {
        protected TwoWayBindableElement(IProperty<TValueType> property)
        {
            Property = property;
        }

        protected IProperty<TValueType> Property { get; }

        public void UpdateValue()
        {
            var propertyValue = Property.Value;

            if (TryGetElementValue(out var elementValue) == false)
            {
                OnPropertyValueChanged(propertyValue);
            }
            else if (EqualityComparer<TValueType>.Default.Equals(elementValue, propertyValue) == false)
            {
                OnPropertyValueChanged(propertyValue);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract bool TryGetElementValue(out TValueType value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void OnPropertyValueChanged(TValueType newValue);
    }
}