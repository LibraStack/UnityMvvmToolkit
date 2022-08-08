using BindableUIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElementWrappers
{
    public class BindableAnimationLabelWrapper : BindablePropertyElement
    {
        private readonly BindableAnimationLabel _animationLabel;
        private readonly IReadOnlyProperty<string> _textProperty;

        public BindableAnimationLabelWrapper(BindableAnimationLabel animationLabel, IObjectProvider objectProvider)
            : base(objectProvider)
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