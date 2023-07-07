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

        private IObjectProvider _objectProvider;
        private ObjectPool<VisualElement> _itemAssetsPool;
        private Dictionary<int, VisualElement> _itemAssets;
        private ObservableCollection<TItemBindingContext> _itemsSource;

        protected PropertyBindingData _itemsSourceBindingData;
        protected IReadOnlyProperty<ObservableCollection<TItemBindingContext>> _itemsSourceProperty;

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
                ClearItems(_itemsSource);
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

            _itemsSourceProperty = objectProvider
                .RentReadOnlyProperty<ObservableCollection<TItemBindingContext>>(context, _itemsSourceBindingData);
            _itemsSource = _itemsSourceProperty.Value;
            _itemsSource.CollectionChanged += OnItemsCollectionChanged;

            AddItems(_itemsSource);
        }

        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_itemsSourceProperty is null)
            {
                return;
            }

            _itemsSource.CollectionChanged -= OnItemsCollectionChanged;

            ClearItems(_itemsSource);

            objectProvider.ReturnReadOnlyProperty(_itemsSourceProperty);

            _itemTemplate = null;
            _objectProvider = null;

            _itemsSource = null;
            _itemsSourceProperty = null;
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

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (_itemsSource.Count == 0)
                {
                    ClearItems();
                }
                else
                {
                    throw new InvalidOperationException("Action not supported.");
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearItems()
        {
            foreach (var (_, item) in _itemAssets)
            {
                _itemsCount--;

                if (_objectProvider is not null)
                {
                    UnbindItem(item, _itemsCount, item.GetBindingContext<TItemBindingContext>(), _objectProvider);
                }

                _itemAssetsPool.Release(item);
            }

            _itemAssets.Clear();
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