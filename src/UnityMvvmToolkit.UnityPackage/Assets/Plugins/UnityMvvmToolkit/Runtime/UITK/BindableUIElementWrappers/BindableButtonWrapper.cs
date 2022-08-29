using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace UnityMvvmToolkit.UITK.BindableUIElementWrappers
{
    public class BindableButtonWrapper : BaseBindableButton
    {
        public BindableButtonWrapper(BindableButton button, IObjectProvider objectProvider) 
            : base(button, objectProvider)
        {
        }
    }
}