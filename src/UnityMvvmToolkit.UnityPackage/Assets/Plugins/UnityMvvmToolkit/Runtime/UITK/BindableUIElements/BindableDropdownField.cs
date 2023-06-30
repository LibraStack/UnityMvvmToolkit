using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public partial class BindableDropdownField : DropdownField, IBindableCollection, IInitializable, IDisposable
    {
        private IProperty<string> _selectedItemProperty;
        private PropertyBindingData _selectedItemBindingData;

        private PropertyBindingData _itemsSourceBindingData;
        private IReadOnlyProperty<ObservableCollection<string>> _itemsSource;

        public void Initialize()
        {
            choices = new List<string>();
        }

        public void Dispose()
        {
            RemoveAllItems();
        }

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            if (string.IsNullOrWhiteSpace(BindingItemsSourcePath))
            {
                return;
            }

            _itemsSourceBindingData ??= BindingItemsSourcePath.ToPropertyBindingData();

            _itemsSource =
                objectProvider.RentReadOnlyProperty<ObservableCollection<string>>(context, _itemsSourceBindingData);
            _itemsSource.Value.CollectionChanged += OnItemsCollectionChanged;

            AddItems(_itemsSource.Value);

            if (string.IsNullOrWhiteSpace(BindingSelectedItemPath) == false)
            {
                _selectedItemBindingData ??= BindingSelectedItemPath.ToPropertyBindingData();

                _selectedItemProperty = objectProvider.RentProperty<string>(context, _selectedItemBindingData);

                if (string.IsNullOrWhiteSpace(_selectedItemProperty.Value))
                {
                    _selectedItemProperty.Value = choices.Count > 0 ? choices[0] : default;
                }
                else
                {
                    UpdateControlValue(_selectedItemProperty.Value);
                }

                _selectedItemProperty.ValueChanged += OnSelectedItemValueChanged;
                this.RegisterValueChangedCallback(OnControlSelectedValueChanged);
            }
        }

        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_itemsSource is null)
            {
                return;
            }

            _itemsSource.Value.CollectionChanged -= OnItemsCollectionChanged;
            objectProvider.ReturnReadOnlyProperty(_itemsSource);
            _itemsSource = null;

            if (_selectedItemProperty is not null)
            {
                _selectedItemProperty.ValueChanged -= OnSelectedItemValueChanged;
                objectProvider.ReturnProperty(_selectedItemProperty);
                _selectedItemProperty = null;

                this.UnregisterValueChangedCallback(OnControlSelectedValueChanged);
            }

            RemoveAllItems();
        }

        protected virtual void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string newItem in e.NewItems)
                {
                    AddItem(newItem);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (string oldItem in e.OldItems)
                {
                    RemoveItem(oldItem);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (_itemsSource.Value.Count == 0)
                {
                    RemoveAllItems();
                }
                else
                {
                    throw new InvalidOperationException("Action not supported.");
                }
            }
        }

        protected virtual void OnControlSelectedValueChanged(ChangeEvent<string> e)
        {
            _selectedItemProperty.Value = e.newValue;
        }

        private void OnSelectedItemValueChanged(object sender, string newValue)
        {
            UpdateControlValue(newValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void UpdateControlValue(string selectedItem)
        {
            if (choices.Any() && choices.Contains(selectedItem) == false)
            {
                throw new InvalidOperationException($"\"{selectedItem}\" is not presented in the collection.");
            }

            SetValueWithoutNotify(selectedItem);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddItems(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddItem(string item)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                throw new NullReferenceException("Item cannot be null or empty.");
            }

            choices.Add(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveItem(string item)
        {
            choices.Remove(item);

            if (value == item)
            {
                value = choices.Count == 0 ? default : choices[^1];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveAllItems()
        {
            choices.Clear();
            value = default;
        }
    }
}