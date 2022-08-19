using Cysharp.Threading.Tasks;
using Extensions;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace BindableUIElements
{
    public class BindablePageBlocker : BindableButton
    {
        public BindablePageBlocker()
        {
            if (Application.isPlaying)
            {
                RegisterCallback<GeometryChangedEvent>(OnLayoutCalculated);
            }
        }

        public async UniTask ActivateAsync()
        {
            visible = true;
            style.opacity = 1;
            await this.WaitForTransitionFinish();
        }
        
        public async UniTask DeactivateAsync()
        {
            style.opacity = 0;
            await this.WaitForTransitionFinish();
            
            visible = false;
        }

        private void OnLayoutCalculated(GeometryChangedEvent e)
        {
            visible = false;
            style.opacity = 0;
            UnregisterCallback<GeometryChangedEvent>(OnLayoutCalculated);
        }

        public new class UxmlFactory : UxmlFactory<BindablePageBlocker, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableButton.UxmlTraits
        {
        }
    }
}