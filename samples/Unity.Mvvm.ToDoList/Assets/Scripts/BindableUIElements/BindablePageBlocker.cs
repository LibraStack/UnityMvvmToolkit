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

        public void Activate()
        {
            visible = true;
        }
        
        public void Deactivate()
        {
            visible = false;
        }

        private void OnLayoutCalculated(GeometryChangedEvent e)
        {
            Deactivate();
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