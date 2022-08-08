using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace UnityMvvmToolkit.UI.BindableUIElementWrappers
{
    public class BindableButtonWrapper : BaseBindableButton
    {
        public BindableButtonWrapper(BindableButton button, IObjectProvider objectProvider) 
            : base(button, objectProvider)
        {
        }
    }
}