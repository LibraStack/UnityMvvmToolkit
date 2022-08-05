using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace UnityMvvmToolkit.UI.BindableVisualElements
{
    public class BindableVisualButton : BindableButtonElement
    {
        public BindableVisualButton(BindableButton button, IObjectProvider objectProvider) 
            : base(button, objectProvider)
        {
        }
    }
}