using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.UniTask;

namespace BindableUIElements
{
    public partial class BindablePageBlocker : BindableBinaryStateButton
    {
        public BindablePageBlocker()
        {
            if (Application.isPlaying)
            {
                RegisterCallback<GeometryChangedEvent>(OnLayoutCalculated);
            }
        }

        public override void Activate()
        {
            visible = true;
            style.opacity = 1;
        }

        public override async void Deactivate()
        {
            try
            {
                style.opacity = 0;
                await this.WaitForLongestTransitionEnd();
            }
            finally
            {
                visible = false;
            }
        }

        private void OnLayoutCalculated(GeometryChangedEvent e)
        {
            visible = false;
            style.opacity = 0;

            UnregisterCallback<GeometryChangedEvent>(OnLayoutCalculated);
        }
    }
}