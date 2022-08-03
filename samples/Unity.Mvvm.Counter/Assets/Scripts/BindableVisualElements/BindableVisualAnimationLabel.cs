using BindableUIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableVisualElements
{
    public class BindableVisualAnimationLabel : BindableVisualElement
    {
        private readonly BindableAnimationLabel _animationLabel;
        private readonly IReadOnlyProperty<string> _textProperty;

        public BindableVisualAnimationLabel(BindableAnimationLabel animationLabel, IPropertyProvider propertyProvider)
            : base(propertyProvider)
        {
            _animationLabel = animationLabel;
            _textProperty = GetReadOnlyProperty<string>(animationLabel.BindingTextPath);
        }

        public override void UpdateValues()
        {
            if (_textProperty != null)
            {
                _animationLabel.SetText(_textProperty.Value);
            }
        }
    }
}