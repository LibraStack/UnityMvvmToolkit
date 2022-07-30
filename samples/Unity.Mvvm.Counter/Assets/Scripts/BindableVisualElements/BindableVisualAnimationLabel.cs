using System.Runtime.CompilerServices;
using BindableUIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableVisualElements
{
    public class BindableVisualAnimationLabel<TValueType> : OneWayBindableElement<TValueType>
    {
        private readonly BindableAnimationLabel _animationLabel;
        private readonly IValueConverter<TValueType, string> _valueConverter;

        public BindableVisualAnimationLabel(BindableAnimationLabel animationLabel,
            IReadOnlyProperty<TValueType> property, IValueConverter<TValueType, string> valueConverter) : base(property)
        {
            _animationLabel = animationLabel;
            _valueConverter = valueConverter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void OnPropertyValueChanged(TValueType newValue)
        {
            if (_valueConverter.TryConvert(newValue, out var result))
            {
                _animationLabel.SetText(result);
            }
        }
    }
}