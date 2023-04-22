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
    public partial class BindableScrollView : ScrollView, IBindableElement, IInitializable, IDisposable
    {
        private PropertyBindingData _itemsSourceBindingData;
        private PropertyBindingData _itemTemplateBindingData;

        private IReadOnlyProperty<VisualTreeAsset> _itemTemplate;
        private IReadOnlyProperty<ObservableCollection<ICollectionItem>> _itemsSource;

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
            _itemAssets.Clear();
            _itemAssetsPool.Dispose();
        }

        public virtual void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _itemsSourceBindingData ??= BindingItemsSourcePath.ToPropertyBindingData();
            _itemTemplateBindingData ??= BindingItemTemplatePath.ToPropertyBindingData();

            _objectProvider = objectProvider;

            _itemsSource =
                objectProvider.RentReadOnlyProperty<ObservableCollection<ICollectionItem>>(context,
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

            objectProvider.ReturnReadOnlyProperty(_itemsSource);
            objectProvider.ReturnReadOnlyProperty(_itemTemplate);

            _itemsSource.Value.CollectionChanged -= OnItemsCollectionChanged;
            _itemsSource = null;

            _itemTemplate = null;
            _objectProvider = null;

            ClearItems();
        }

        protected virtual void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    AddItem((ICollectionItem) newItem);
                }

                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    RemoveItem((ICollectionItem) oldItem);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddItems(IEnumerable<ICollectionItem> items)
        {
            foreach (var itemBindingContext in items)
            {
                AddItem(itemBindingContext);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddItem(ICollectionItem itemBindingContext)
        {
            if (itemBindingContext is IInitializable initializable)
            {
                initializable.Initialize();
            }

            var item = _itemAssetsPool.Get();
            item.SetBindingContext(itemBindingContext, _objectProvider, true);

            _itemAssets.Add(itemBindingContext.Id, item);
            contentContainer.Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveItem(ICollectionItem itemBindingContext)
        {
            if (itemBindingContext is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _itemAssetsPool.Release(_itemAssets[itemBindingContext.Id]);
            _itemAssets.Remove(itemBindingContext.Id);
        }

        private void ClearItems()
        {
            _itemAssets.Clear();
            Clear();
        }

        private VisualElement OnPoolInstantiateItem()
        {
            return _itemTemplate.Value.InstantiateBindableElement();
        }

        private void OnPooReleaseItem(VisualElement item)
        {
            if (_objectProvider != null)
            {
                item.ResetBindingContext(_objectProvider, true);
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