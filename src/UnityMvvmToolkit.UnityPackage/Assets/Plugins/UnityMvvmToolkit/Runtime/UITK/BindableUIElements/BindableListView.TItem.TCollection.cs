using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public abstract partial class BindableListView<TItemBindingContext, TCollection> : 
        ListView, IBindableCollection, IInitializable, IDisposable
        where TItemBindingContext : ICollectionItem
        where TCollection : IList<TItemBindingContext>, IList, INotifyCollectionChanged
    {
        private VisualTreeAsset _itemTemplate;

        private IObjectProvider _objectProvider;
        private List<VisualElement> _itemAssets;
        private IList<TItemBindingContext> _itemsSource;
        private Dictionary<int, TItemBindingContext> _activeItems;

        protected PropertyBindingData _itemsSourceBindingData;
        protected IReadOnlyProperty<TCollection> _itemsSourceProperty;

        public virtual void Initialize()
        {
            _itemAssets = new List<VisualElement>();
            _activeItems = new Dictionary<int, TItemBindingContext>();
        }

        public virtual void Dispose()
        {
            for (var i = 0; i < _itemAssets.Count; i++)
            {
                _itemAssets[i].DisposeBindableElement(_objectProvider);
            }

            _itemAssets.Clear();
            _activeItems.Clear();
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

            _itemsSourceProperty = objectProvider.RentReadOnlyProperty<TCollection>(context, _itemsSourceBindingData);
            _itemsSourceProperty.Value.CollectionChanged += OnItemsCollectionChanged;
            _itemsSource = _itemsSourceProperty.Value;

            itemsSource = _itemsSourceProperty.Value;
            makeItem += OnMakeItem;
            bindItem += OnBindItem;
            unbindItem += OnUnbindItem;
        }

        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_itemsSourceProperty is null)
            {
                return;
            }

            _itemsSourceProperty.Value.CollectionChanged -= OnItemsCollectionChanged;

            makeItem -= OnMakeItem;
            bindItem -= OnBindItem;
            unbindItem -= OnUnbindItem;
            itemsSource = Array.Empty<TItemBindingContext>();

            objectProvider.ReturnReadOnlyProperty(_itemsSourceProperty);

            _itemTemplate = null;
            _objectProvider = null;

            _itemsSource = default;
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
#if UNITY_2021
            if (e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Reset)
            {
                Rebuild();
                return;
            }
#endif
            RefreshItems();
        }

        private VisualElement OnMakeItem()
        {
            var item = MakeItem(_itemTemplate);

            _itemAssets.Add(item);

            return item;
        }

        private void OnBindItem(VisualElement item, int index)
        {
            var itemId = item.GetHashCode();
            var itemBindingContext = _itemsSource[index];

            if (_activeItems.TryAdd(itemId, itemBindingContext))
            {
                BindItem(item, index, itemBindingContext, _objectProvider);
            }
        }

        private void OnUnbindItem(VisualElement item, int index)
        {
            var itemId = item.GetHashCode();

            if (_activeItems.Remove(itemId, out var itemBindingContext))
            {
                UnbindItem(item, index, itemBindingContext, _objectProvider);
            }
        }
    }
}