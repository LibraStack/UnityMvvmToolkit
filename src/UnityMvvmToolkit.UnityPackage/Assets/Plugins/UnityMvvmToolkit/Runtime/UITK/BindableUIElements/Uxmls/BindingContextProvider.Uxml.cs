using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindingContextProvider
    {
        public new class UxmlFactory : UxmlFactory<BindingContextProvider, UxmlTraits>
        {
        }

#if UNITY_2023_2_OR_NEWER
        [System.Serializable]
        public new class UxmlSerializedData : BindingContextProvider<IBindingContext>.UxmlSerializedData
        {
            public override object CreateInstance() => new BindingContextProvider();
        }
#else
        public new class UxmlTraits : BindingContextProvider<IBindingContext>.UxmlTraits
        {
        }
#endif
    }
}