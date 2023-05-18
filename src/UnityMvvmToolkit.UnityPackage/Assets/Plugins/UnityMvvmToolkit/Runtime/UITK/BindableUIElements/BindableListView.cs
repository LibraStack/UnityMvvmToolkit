using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public abstract partial class BindableListView<TItemBindingContext> : ListView, IBindableElement
        where TItemBindingContext : ICollectionItem
    {
        private VisualTreeAsset _itemTemplate;

        private PropertyBindingData _itemsSourceBindingData;
        private IReadOnlyProperty<ObservableCollection<TItemBindingContext>> _itemsSource;

        private IObjectProvider _objectProvider;

        public virtual void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _itemsSourceBindingData ??= BindingItemsSourcePath.ToPropertyBindingData();
            _itemTemplate ??= objectProvider.GetCollectionItemTemplate<TItemBindingContext, VisualTreeAsset>();

            _objectProvider = objectProvider;

            _itemsSource =
                objectProvider.RentReadOnlyProperty<ObservableCollection<TItemBindingContext>>(context,
                    _itemsSourceBindingData);
            _itemsSource.Value.CollectionChanged += OnItemsCollectionChanged;

            itemsSource = _itemsSource.Value;
            makeItem += MakeItem;
            bindItem += BindItem;
            unbindItem += UnbindItem;
        }

        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_itemsSource == null)
            {
                return;
            }

            _itemsSource.Value.CollectionChanged -= OnItemsCollectionChanged;

            makeItem -= MakeItem;
            bindItem -= BindItem;
            unbindItem -= UnbindItem;

            ClearItems();

            objectProvider.ReturnReadOnlyProperty(_itemsSource);

            _itemsSource = null;
            _itemTemplate = null;
            _objectProvider = null;
        }

        protected virtual VisualElement MakeItem(VisualTreeAsset itemTemplate)
        {
            return itemTemplate.InstantiateBindableElement(); // TODO: Pool.
        }

        protected virtual void BindItem(VisualElement item, int index, TItemBindingContext bindingContext,
            IObjectProvider objectProvider)
        {
            item.SetBindingContext(bindingContext, objectProvider, true);
        }

        protected virtual void UnbindItem(VisualElement item, int index, TItemBindingContext bindingContext,
            IObjectProvider objectProvider)
        {
            item.ResetBindingContext(objectProvider, true);
        }

        private VisualElement MakeItem()
        {
            return MakeItem(_itemTemplate);
        }

        private void BindItem(VisualElement item, int index)
        {
            if (index >= 0 && index < itemsSource.Count)
            {
                BindItem(item, index, _itemsSource.Value[index], _objectProvider);
            }
            else
            {
                BindItem(item, index, default, _objectProvider);
            }
        }

        private void UnbindItem(VisualElement item, int index)
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

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshItems(); // TODO: Do not refresh all items.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearItems()
        {
            itemsSource = Array.Empty<TItemBindingContext>();
        }
    }
}