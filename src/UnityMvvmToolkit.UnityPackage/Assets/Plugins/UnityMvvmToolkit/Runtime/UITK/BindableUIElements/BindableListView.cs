using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        private PropertyBindingData _itemTemplateBindingData;
        private IReadOnlyProperty<VisualTreeAsset> _itemTemplate;

        private PropertyBindingData _itemsSourceBindingData;
        private IReadOnlyProperty<ObservableCollection<TItemBindingContext>> _itemsSource;

        private IObjectProvider _objectProvider;

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

            objectProvider.ReturnReadOnlyProperty(_itemsSource);
            objectProvider.ReturnReadOnlyProperty(_itemTemplate);

            makeItem -= MakeItem;
            bindItem -= BindItem;
            unbindItem -= UnbindItem;
            Clear();

            _itemsSource = null;
            _itemTemplate = null;
            _objectProvider = null;
        }

        protected virtual VisualElement MakeItem(VisualTreeAsset itemTemplate)
        {
            return itemTemplate.InstantiateBindableElement(); // TODO: Pool.
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

        private VisualElement MakeItem()
        {
            return MakeItem(_itemTemplate.Value);
        }

        private void BindItem(VisualElement item, int index)
        {
            BindItem(item, _itemsSource.Value[index], _objectProvider);
        }

        private void UnbindItem(VisualElement item, int index)
        {
            UnbindItem(item, _objectProvider);
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshItems(); // TODO: Do not refresh all items.
        }
    }
}