using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public abstract partial class BindableScrollView<TItemBindingContext> : ScrollView, IBindableElement,
        IInitializable, IDisposable where TItemBindingContext : ICollectionItem
    {
        private PropertyBindingData _itemTemplateBindingData;
        private IReadOnlyProperty<VisualTreeAsset> _itemTemplate;

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
            ClearItems();
            _itemAssetsPool.Dispose();
        }

        public virtual void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _itemsSourceBindingData ??= BindingItemsSourcePath.ToPropertyBindingData();
            _itemTemplateBindingData ??= BindingItemTemplatePath.ToPropertyBindingData();

            _objectProvider = objectProvider;

            _itemsSource =
                objectProvider.RentReadOnlyProperty<ObservableCollection<TItemBindingContext>>(context,
                    _itemsSourceBindingData);
            _itemsSource.Value.CollectionChanged += OnItemsCollectionChanged;

            _itemTemplate = objectProvider.RentReadOnlyProperty<VisualTreeAsset>(context, _itemTemplateBindingData);

            AddItems(_itemsSource.Value);
        }

        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_itemsSource == null)
            {
                return;
            }

            _itemsSource.Value.CollectionChanged -= OnItemsCollectionChanged;

            objectProvider.ReturnReadOnlyProperty(_itemsSource);
            objectProvider.ReturnReadOnlyProperty(_itemTemplate);

            ClearItems();

            _itemsSource = null;
            _itemTemplate = null;
            _objectProvider = null;
        }

        protected virtual VisualElement MakeItem(VisualTreeAsset itemTemplate)
        {
            return itemTemplate.InstantiateBindableElement();
        }

        protected virtual void BindItem(VisualElement item, TItemBindingContext bindingContext,
            IObjectProvider objectProvider)
        {
            item.SetBindingContext(bindingContext, objectProvider, true);
        }

        protected virtual void UnbindItem(VisualElement item, IObjectProvider objectProvider)
        {
            item.ResetBindingContext(objectProvider, true);
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            BindItem(item, itemBindingContext, _objectProvider);

            _itemAssets.Add(itemBindingContext.Id, item);
            contentContainer.Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveItem(TItemBindingContext itemBindingContext)
        {
            _itemAssetsPool.Release(_itemAssets[itemBindingContext.Id]);
            _itemAssets.Remove(itemBindingContext.Id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearItems()
        {
            if (_itemAssets.Count == 0)
            {
                return;
            }

            foreach (var asset in _itemAssets)
            {
                _itemAssetsPool.Release(asset.Value);
            }

            _itemAssets.Clear();
        }

        private VisualElement OnPoolInstantiateItem()
        {
            return MakeItem(_itemTemplate.Value);
        }

        private void OnPooReleaseItem(VisualElement item)
        {
            if (_objectProvider != null)
            {
                UnbindItem(item, _objectProvider);
            }

            item.RemoveFromHierarchy();
        }

        private void OnPoolDestroyItem(VisualElement item)
        {
            if (_objectProvider != null)
            {
                item.ResetBindingContext(_objectProvider, true);
            }
        }
    }
}