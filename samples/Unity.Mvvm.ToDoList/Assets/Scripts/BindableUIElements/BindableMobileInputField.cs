using UnityEngine;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace BindableUIElements
{
    public partial class BindableMobileInputField : BindableTextField
    {
        public bool HideMobileInput
        {
            get => TouchScreenKeyboard.hideInput;
            set => TouchScreenKeyboard.hideInput = value;
        }
    }
}