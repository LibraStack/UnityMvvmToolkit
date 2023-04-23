using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public abstract class BaseBindableListView<TItemBindingContext> : ListView, IBindableElement
    {
        private IReadOnlyProperty<VisualTreeAsset> _itemTemplate;
        private IReadOnlyProperty<ObservableCollection<ICollectionItem>> _itemsSource;

        private IObjectProvider _objectProvider;

        public virtual void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _objectProvider = objectProvider;

            _itemsSource =
                objectProvider.RentReadOnlyProperty<ObservableCollection<ICollectionItem>>(context,
                    GetItemsSourceBindingData());
            _itemsSource.Value.CollectionChanged += OnItemsCollectionChanged;

            _itemTemplate = objectProvider.RentReadOnlyProperty<VisualTreeAsset>(context, GetItemTemplateBindingData());

            itemsSource = _itemsSource.Value;
            makeItem += MakeItem;
            bindItem += BindItem;
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

            makeItem -= MakeItem;
            bindItem -= BindItem;
            unbindItem -= UnbindItem;
            Clear();
        }

        protected virtual void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshItems(); // TODO: Do not refresh all items.
        }

        protected virtual VisualElement MakeItem()
        {
            return MakeItem(_itemTemplate.Value);
        }

        protected virtual void BindItem(VisualElement item, int index)
        {
            if (index >= 0 && index < itemsSource.Count)
            {
                BindItem(item, _itemsSource.Value[index], _objectProvider);
            }
        }

        protected virtual void UnbindItem(VisualElement item, int index)
        {
            if (index >= 0 && index < itemsSource.Count)
            {
                UnbindItem(item, _objectProvider);
            }
        }

        protected abstract PropertyBindingData GetItemsSourceBindingData();
        protected abstract PropertyBindingData GetItemTemplateBindingData();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static VisualElement MakeItem(VisualTreeAsset itemAsset)
        {
            return itemAsset.InstantiateBindableElement(); // TODO: Pool.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BindItem(VisualElement item, IBindingContext bindingContext, IObjectProvider objectProvider)
        {
            item.SetBindingContext(bindingContext, objectProvider, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UnbindItem(VisualElement item, IObjectProvider objectProvider)
        {
            item.ResetBindingContext(objectProvider, true);
        }
    }
}