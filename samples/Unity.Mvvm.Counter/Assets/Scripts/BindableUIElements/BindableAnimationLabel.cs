using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine.UIElements;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace BindableUIElements
{
    public abstract class BindableAnimationLabel : BindableLabel
    {
        private bool _isLayoutCalculated;

        protected BindableAnimationLabel()
        {
            RegisterCallback<GeometryChangedEvent>(OnLayoutCalculated);
        }

        protected abstract ILabelAnimation Animation { get; }

        public void SetText(string result, bool playAnimation = true)
        {
            text = result;

            if (_isLayoutCalculated && playAnimation)
            {
                Animation?.PlayAsync().Forget();
            }
        }

        private void OnLayoutCalculated(GeometryChangedEvent e)
        {
            _isLayoutCalculated = true;
        }
    }
}