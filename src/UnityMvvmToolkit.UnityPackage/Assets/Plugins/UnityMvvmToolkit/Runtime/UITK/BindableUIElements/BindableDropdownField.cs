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
        private IReadOnlyProperty<ObservableCollection<string>> _itemsSource;

        private PropertyBindingData _selectedItemBindingData;
        private PropertyBindingData _itemsSourceBindingData;

        public void Initialize()
        {
            choices = new List<string>();
        }

        public void Dispose()
        {
            choices.Clear();
        }
        
        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            if (string.IsNullOrWhiteSpace(BindingItemsSourcePath) == false)
            {
                _itemsSourceBindingData ??= BindingItemsSourcePath.ToPropertyBindingData();
                _itemsSource = objectProvider
                    .RentReadOnlyProperty<ObservableCollection<string>>(context, _itemsSourceBindingData);
                _itemsSource.Value.CollectionChanged += OnItemsCollectionChanged;
                choices = new List<string>(_itemsSource.Value);
            }
            
            if (string.IsNullOrWhiteSpace(BindingSelectedItemPath) == false)
            {
                _selectedItemBindingData ??= BindingSelectedItemPath.ToPropertyBindingData();
                _selectedItemProperty = objectProvider.RentProperty<string>(context, _selectedItemBindingData);
                _selectedItemProperty.ValueChanged += OnSelectedItemValueChanged;
                
                var isContains = choices.Contains(_selectedItemProperty.Value);
                if (isContains == true)
                {
                    UpdateControlValue(_selectedItemProperty.Value);
                }
                
                this.RegisterValueChangedCallback(OnControlValueChanged);
                _selectedItemProperty.Value = choices.Count > 0 ? choices[0] : default;
            }
        }

        protected virtual void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string newItem in e.NewItems)
                {
                    choices.Add(newItem);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (string oldItem in e.OldItems)
                {
                    choices.Remove(oldItem);
                }
            }
        }
        
        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_selectedItemProperty != null)
            {
                _selectedItemProperty.ValueChanged -= OnSelectedItemValueChanged;
                objectProvider.ReturnProperty(_selectedItemProperty);
                _selectedItemProperty = null;
                this.UnregisterValueChangedCallback(OnControlValueChanged);
            }
            
            if (_itemsSource != null)
            {
                _itemsSource.Value.CollectionChanged -= OnItemsCollectionChanged;
                choices = new List<string>();
                objectProvider.ReturnReadOnlyProperty(_itemsSource);
                _itemsSource = null;
            }

            UpdateControlValue(default);
        }

        protected virtual void OnControlValueChanged(ChangeEvent<string> e)
        {
            _selectedItemProperty.Value = e.newValue;
        }

        private void OnSelectedItemValueChanged(object sender, string newValue)
        {
            var isContains = choices.Contains(newValue);
            if (isContains == false)
            {
                return;
            }
            
            UpdateControlValue(newValue);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void UpdateControlValue(string newValue)
        {
            SetValueWithoutNotify(newValue);
        }
    }
}