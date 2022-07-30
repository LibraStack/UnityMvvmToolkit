using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace UnityMvvmToolkit.UI.BindableVisualElements
{
    public class BindableVisualLabel<TValueType> : OneWayBindableElement<TValueType>
    {
        private readonly BindableLabel _label;
        private readonly IValueConverter<TValueType, string> _valueConverter;

        public BindableVisualLabel(BindableLabel label, IReadOnlyProperty<TValueType> property,
            IValueConverter<TValueType, string> valueConverter) : base(property)
        {
            _label = label;
            _valueConverter = valueConverter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void OnPropertyValueChanged(TValueType newValue)
        {
            if (_valueConverter.TryConvert(newValue, out var result))
            {
                _label.text = result;
            }
        }
    }
}