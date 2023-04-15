using Cysharp.Threading.Tasks;
using Interfaces;
using LabelAnimations;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace BindableUIElements
{
    public partial class BindableCountLabel : BindableLabel
    {
        private ILabelAnimation _scaleAnimation;

        public BindableCountLabel()
        {
            RegisterCallback<GeometryChangedEvent>(OnLayoutCalculated);
        }

        protected override void UpdateControlText(string newText)
        {
            base.UpdateControlText(newText);
            _scaleAnimation?.PlayAsync().Forget();
        }

        private void OnLayoutCalculated(GeometryChangedEvent e)
        {
            try
            {
                _scaleAnimation ??= new LabelScaleAnimation(this, Vector3.zero, Vector3.one);
            }
            finally
            {
                UnregisterCallback<GeometryChangedEvent>(OnLayoutCalculated);
            }
        }
    }
}