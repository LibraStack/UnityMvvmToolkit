using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public abstract partial class BindableScrollView<TItemBindingContext> : ScrollView, IBindableCollection,
        IInitializable, IDisposable where TItemBindingContext : ICollectionItem
    {
        private int _itemsCount;

        private VisualTreeAsset _itemTemplate;

        private PropertyBindingData _itemsSourceBindingData;
        private IReadOnlyProperty<ObservableCollection<TItemBindingContext>> _itemsSource;

        private IObjectProvider _objectProvider;

        private ObjectPool<VisualElement> _itemAssetsPool;
        private Dictionary<int, VisualElement> _itemAssets;

        public virtual void Initialize()
        {
            _itemAssets = new Dictionary<int, VisualElement>();
            _itemAssetsPool = new ObjectPool<VisualElement>(OnPoolInstantiateItem, actionOnRelease: OnPooReleaseItem,
                actionOnDestroy: OnPoolDestroyItem);
        }

        public virtual void Dispose()
        {
            if (_itemsSource is null)
            {
                _itemsCount = 0;
                _itemAssets.Clear();
            }
            else
            {
                ClearItems(_itemsSource.Value);
            }

            _itemAssetsPool.Dispose();
        }

        public virtual void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            if (string.IsNullOrWhiteSpace(BindingItemsSourcePath))
            {
                return;
            }

            _itemsSourceBindingData ??= BindingItemsSourcePath.ToPropertyBindingData();
            _itemTemplate ??= objectProvider.GetCollectionItemTemplate<TItemBindingContext, VisualTreeAsset>();

            _objectProvider = objectProvider;

            _itemsSource = objectProvider
                .RentReadOnlyProperty<ObservableCollection<TItemBindingContext>>(context, _itemsSourceBindingData);
            _itemsSource.Value.CollectionChanged += OnItemsCollectionChanged;

            AddItems(_itemsSource.Value);
        }

        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_itemsSource is null)
            {
                return;
            }

            _itemsSource.Value.CollectionChanged -= OnItemsCollectionChanged;

            ClearItems(_itemsSource.Value);

            objectProvider.ReturnReadOnlyProperty(_itemsSource);

            _itemsSource = null;
            _itemTemplate = null;
            _objectProvider = null;
        }

        protected virtual VisualElement MakeItem(VisualTreeAsset itemTemplate)
        {
            return itemTemplate
                .InstantiateBindableElement()
                .InitializeBindableElement();
        }

        protected virtual void BindItem(VisualElement item, int index, TItemBindingContext bindingContext,
            IObjectProvider objectProvider)
        {
            item.SetChildsBindingContext(bindingContext, objectProvider);
        }

        protected virtual void UnbindItem(VisualElement item, int index, TItemBindingContext bindingContext,
            IObjectProvider objectProvider)
        {
            item.ResetChildsBindingContext(objectProvider);
        }

        protected virtual void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    AddItem((TItemBindingContext) newItem);
                }

                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    RemoveItem((TItemBindingContext) oldItem);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddItems(IEnumerable<TItemBindingContext> items)
        {
            foreach (var itemBindingContext in items)
            {
                AddItem(itemBindingContext);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddItem(TItemBindingContext itemBindingContext)
        {
            var item = _itemAssetsPool.Get();
            BindItem(item, _itemsCount, itemBindingContext, _objectProvider);

            _itemsCount++;
            _itemAssets.Add(itemBindingContext.Id, item);

            contentContainer.Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveItem(TItemBindingContext itemBindingContext)
        {
            _itemsCount--;

            var item = _itemAssets[itemBindingContext.Id];

            if (_objectProvider is not null)
            {
                UnbindItem(item, _itemsCount, itemBindingContext, _objectProvider);
            }

            _itemAssets.Remove(itemBindingContext.Id);
            _itemAssetsPool.Release(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearItems(IReadOnlyList<TItemBindingContext> items)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                RemoveItem(items[i]);
            }
        }

        private VisualElement OnPoolInstantiateItem()
        {
            return MakeItem(_itemTemplate);
        }

        private void OnPooReleaseItem(VisualElement item)
        {
            item.RemoveFromHierarchy();
        }

        private void OnPoolDestroyItem(VisualElement item)
        {
            item.DisposeBindableElement(_objectProvider);
        }
    }
}