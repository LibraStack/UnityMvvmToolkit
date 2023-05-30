using UnityEngine.UIElements;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public abstract partial class BaseButton : Button
    {
        public bool Enabled
        {
            get => enabledSelf;
            set => SetEnabled(value);
        }
    }
}