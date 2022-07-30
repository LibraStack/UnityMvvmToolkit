using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common
{
    public abstract class OneWayBindableElement<TValueType> : IBindableVisualElement
    {
        private readonly IReadOnlyProperty<TValueType> _property;

        protected OneWayBindableElement(IReadOnlyProperty<TValueType> property)
        {
            _property = property;
        }

        public void UpdateValue()
        {
            OnPropertyValueChanged(_property.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void OnPropertyValueChanged(TValueType newValue);
    }
}