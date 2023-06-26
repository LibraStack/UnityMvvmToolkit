using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK.Extensions;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public abstract partial class BindableListView<TItemBindingContext> : ListView, IBindableCollection,
        IInitializable, IDisposable where TItemBindingContext : ICollectionItem
    {
        private VisualTreeAsset _itemTemplate;

        private PropertyBindingData _itemsSourceBindingData;
        private ObservableCollection<TItemBindingContext> _itemsSource;
        private IReadOnlyProperty<ObservableCollection<TItemBindingContext>> _itemsSourceProperty;

        private IObjectProvider _objectProvider;
        private List<VisualElement> _itemAssets;
        private Dictionary<int, TItemBindingContext> _activeItems;

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

            _itemsSourceProperty = objectProvider
                .RentReadOnlyProperty<ObservableCollection<TItemBindingContext>>(context, _itemsSourceBindingData);
            _itemsSource = _itemsSourceProperty.Value;
            _itemsSource.CollectionChanged += OnItemsCollectionChanged;

            itemsSource = _itemsSource;
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

            _itemsSource.CollectionChanged -= OnItemsCollectionChanged;

            makeItem -= OnMakeItem;
            bindItem -= OnBindItem;
            unbindItem -= OnUnbindItem;
            itemsSource = Array.Empty<TItemBindingContext>();

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

            _activeItems.Add(itemId, itemBindingContext);

            BindItem(item, index, itemBindingContext, _objectProvider);
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