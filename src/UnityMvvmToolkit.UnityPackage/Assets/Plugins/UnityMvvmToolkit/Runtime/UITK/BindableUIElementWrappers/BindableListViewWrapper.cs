using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace UnityMvvmToolkit.UITK.BindableUIElementWrappers
{
    public abstract class BindableListViewWrapper<TItem, TData> : IBindableElement, IInitializable, IDisposable
    {
        private readonly BindableListView _listView;
        private readonly VisualTreeAsset _itemAsset;
        private readonly IReadOnlyProperty<ObservableCollection<TData>> _itemsCollectionProperty;

        private ObservableCollection<TData> _itemsCollection;

        protected BindableListViewWrapper(BindableListView listView, VisualTreeAsset itemAsset,
            IObjectProvider objectProvider)
        {
            _listView = listView;
            _itemAsset = itemAsset;
            _itemsCollectionProperty =
                objectProvider.GetReadOnlyProperty<ObservableCollection<TData>>(listView.BindingItemsSourcePath,
                    ReadOnlyMemory<char>.Empty);
        }

        public bool CanInitialize => _itemsCollectionProperty != null;

        protected BindableListView ListView => _listView;
        protected ObservableCollection<TData> ItemsCollection => _itemsCollection;

        public virtual void Initialize()
        {
            _itemsCollection = _itemsCollectionProperty.Value;
            _itemsCollection.CollectionChanged += OnItemsCollectionChanged;

            _listView.itemsSource = _itemsCollection;
            _listView.makeItem += MakeItem;
            _listView.bindItem += BindItem;
        }

        public virtual void Dispose()
        {
            _itemsCollection.CollectionChanged -= OnItemsCollectionChanged;

            _listView.makeItem -= MakeItem;
            _listView.bindItem -= BindItem;
        }

        protected virtual VisualElement MakeItem()
        {
            var itemAsset = _itemAsset.Instantiate();
            itemAsset.userData = OnMakeItem(itemAsset);

            return itemAsset;
        }

        protected virtual void BindItem(VisualElement itemAsset, int index)
        {
            if (index >= 0 && index < _itemsCollection.Count)
            {
                OnBindItem((TItem) itemAsset.userData, _itemsCollection[index]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract TItem OnMakeItem(VisualElement itemAsset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void OnBindItem(TItem item, TData data);

        protected virtual void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _listView.RefreshItems();
        }
    }
}