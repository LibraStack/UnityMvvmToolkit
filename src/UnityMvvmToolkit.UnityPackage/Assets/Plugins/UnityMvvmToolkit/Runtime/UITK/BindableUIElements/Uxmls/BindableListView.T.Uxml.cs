using System.Collections.ObjectModel;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    partial class BindableListView<TItemBindingContext> where TItemBindingContext : ICollectionItem
    {
#if UNITY_2023_2_OR_NEWER
        [System.Serializable]
        public new class UxmlSerializedData : 
            BindableListView<TItemBindingContext, ObservableCollection<TItemBindingContext>>.UxmlSerializedData
        {
        }
#else
        public new class UxmlTraits : 
            BindableListView<TItemBindingContext, ObservableCollection<TItemBindingContext>>.UxmlTraits
        {
        }
#endif
    }
}