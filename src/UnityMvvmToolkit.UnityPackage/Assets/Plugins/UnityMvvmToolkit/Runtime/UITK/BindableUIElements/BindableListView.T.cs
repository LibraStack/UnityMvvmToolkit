using System.Collections.ObjectModel;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public abstract partial class BindableListView<TItemBindingContext> : 
        BindableListView<TItemBindingContext, ObservableCollection<TItemBindingContext>>
        where TItemBindingContext : ICollectionItem
    {
    }
}