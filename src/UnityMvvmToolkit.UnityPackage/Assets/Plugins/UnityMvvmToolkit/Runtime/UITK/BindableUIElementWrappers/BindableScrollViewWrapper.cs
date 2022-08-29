using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.BindableUIElements;

namespace UnityMvvmToolkit.UITK.BindableUIElementWrappers
{
    public abstract class BindableScrollViewWrapper<TItem, TData> : IBindableElement, IInitializable, IDisposable
        where TData : ICollectionItemData
    {
        private readonly BindableScrollView _scrollView;
        private readonly VisualTreeAsset _itemAsset;
        private readonly IReadOnlyProperty<ObservableCollection<TData>> _itemsCollectionProperty;

        private ObservableCollection<TData> _itemsCollection;
        private ObjectPool<VisualElement> _itemAssetsPool;
        private Dictionary<Guid, VisualElement> _itemAssets;

        protected BindableScrollViewWrapper(BindableScrollView scrollView, VisualTreeAsset itemAsset,
            IObjectProvider objectProvider)
        {
            _scrollView = scrollView;
            _itemAsset = itemAsset;

            _itemsCollectionProperty =
                objectProvider.GetReadOnlyProperty<ObservableCollection<TData>>(scrollView.BindingItemsSourcePath,
                    ReadOnlyMemory<char>.Empty);
        }

        public bool CanInitialize => _itemsCollectionProperty != null;

        protected BindableScrollView ScrollView => _scrollView;
        protected ObservableCollection<TData> ItemsCollection => _itemsCollection;

        public virtual void Initialize()
        {
            _itemAssets = new Dictionary<Guid, VisualElement>();
            _itemAssetsPool = new ObjectPool<VisualElement>(createFunc: OnAssetsPoolCreateItem,
                actionOnRelease: OnAssetsPoolReleaseItem, actionOnDestroy: OnAssetsPoolDestroyItem);

            _itemsCollection = _itemsCollectionProperty.Value;
            _itemsCollection.CollectionChanged += OnItemsCollectionChanged;

            AddItems(_itemsCollection);
        }

        public virtual void Dispose()
        {
            _itemAssets.Clear();
            _itemAssetsPool.Dispose();
            _itemsCollection.CollectionChanged -= OnItemsCollectionChanged;
        }

        protected virtual VisualElement MakeItem(out TItem item)
        {
            var itemAsset = _itemAssetsPool.Get();
            if (itemAsset.userData != null)
            {
                item = (TItem) itemAsset.userData;
                return itemAsset;
            }

            item = OnMakeItem(itemAsset);
            itemAsset.userData = item;

            return itemAsset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract TItem OnMakeItem(VisualElement itemAsset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void OnBindItem(TItem item, TData data);

        protected virtual void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    AddItem((TData) newItem);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    RemoveItem((TData) oldItem);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddItems(IEnumerable<TData> items)
        {
            foreach (var itemData in items)
            {
                AddItem(itemData);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddItem(TData itemData)
        {
            var itemAsset = MakeItem(out var item);

            _itemAssets.Add(itemData.Id, itemAsset);
            _scrollView.contentContainer.Add(itemAsset);

            OnBindItem(item, itemData);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveItems(IEnumerable<TData> items)
        {
            foreach (var itemData in items)
            {
                RemoveItem(itemData);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveItem(TData itemData)
        {
            _itemAssetsPool.Release(_itemAssets[itemData.Id]);
        }

        private VisualElement OnAssetsPoolCreateItem()
        {
            return _itemAsset.Instantiate();
        }

        private void OnAssetsPoolReleaseItem(VisualElement itemAsset)
        {
            itemAsset.RemoveFromHierarchy();
        }

        private void OnAssetsPoolDestroyItem(VisualElement itemAsset)
        {
            if (itemAsset.userData is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}