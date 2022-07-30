using Interfaces;
using LabelAnimations;
using UnityEngine;
using UnityEngine.UIElements;

namespace BindableUIElements
{
    public class BindableCountLabel : BindableAnimationLabel
    {
        private ILabelAnimation _scaleAnimation;

        protected override ILabelAnimation Animation
        {
            get { return _scaleAnimation ??= new LabelScaleAnimation(this, Vector3.zero, Vector3.one); }
        }

        public new class UxmlFactory : UxmlFactory<BindableCountLabel, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableAnimationLabel.UxmlTraits
        {
        }
    }
}