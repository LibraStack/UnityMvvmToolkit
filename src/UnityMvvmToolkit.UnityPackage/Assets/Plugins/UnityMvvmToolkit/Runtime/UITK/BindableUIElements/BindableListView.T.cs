using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using JetBrains.Annotations;
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
        private IReadOnlyProperty<ObservableCollection<TItemBindingContext>> _itemsSource;

        private IObjectProvider _objectProvider;
        private List<VisualElement> _itemAssets;

        public virtual void Initialize()
        {
            _itemAssets = new List<VisualElement>();
        }

        public virtual void Dispose()
        {
            for (var i = 0; i < _itemAssets.Count; i++)
            {
                _itemAssets[i].DisposeBindableElement(_objectProvider);
            }

            _itemAssets.Clear();
        }

        public virtual void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _itemsSourceBindingData ??= BindingItemsSourcePath.ToPropertyBindingData();
            _itemTemplate ??= objectProvider.GetCollectionItemTemplate<TItemBindingContext, VisualTreeAsset>();

            _objectProvider = objectProvider;

            _itemsSource = objectProvider
                .RentReadOnlyProperty<ObservableCollection<TItemBindingContext>>(context, _itemsSourceBindingData);
            _itemsSource.Value.CollectionChanged += OnItemsCollectionChanged;

            itemsSource = _itemsSource.Value;
            makeItem += OnMakeItem;
            bindItem += OnBindItem;
            unbindItem += OnUnbindItem;
        }

        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_itemsSource is null)
            {
                return;
            }

            _itemsSource.Value.CollectionChanged -= OnItemsCollectionChanged;

            makeItem -= OnMakeItem;
            bindItem -= OnBindItem;
            unbindItem -= OnUnbindItem;
            itemsSource = Array.Empty<TItemBindingContext>();

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

        protected virtual void UnbindItem(VisualElement item, int index, [CanBeNull] TItemBindingContext bindingContext,
            IObjectProvider objectProvider)
        {
            item.ResetChildsBindingContext(objectProvider);
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshItems(); // TODO: Do not refresh all items.
        }

        private VisualElement OnMakeItem()
        {
            var item = MakeItem(_itemTemplate);
            _itemAssets.Add(item);

            return item;
        }

        private void OnBindItem(VisualElement item, int index)
        {
            BindItem(item, index, _itemsSource.Value[index], _objectProvider);
        }

        private void OnUnbindItem(VisualElement item, int index)
        {
            if (index >= 0 && index < itemsSource.Count)
            {
                UnbindItem(item, index, _itemsSource.Value[index], _objectProvider);
            }
            else
            {
                UnbindItem(item, index, default, _objectProvider);
            }
        }
    }
}