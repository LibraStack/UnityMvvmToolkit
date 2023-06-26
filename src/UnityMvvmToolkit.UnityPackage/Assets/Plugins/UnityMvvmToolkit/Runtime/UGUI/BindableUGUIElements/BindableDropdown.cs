#if UNITYMVVMTOOLKIT_TEXTMESHPRO_SUPPORT

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UGUI.BindableUGUIElements
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class BindableDropdown : MonoBehaviour, IBindableElement
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private string _bindingSelectedItemPath;
        [SerializeField] private string _bindingItemsSourcePath;

        private IProperty<string> _selectedItemProperty;
        private IReadOnlyProperty<ObservableCollection<string>> _itemsSource;

        private PropertyBindingData _selectedItemBindingData;
        private PropertyBindingData _itemsSourceBindingData;
        
        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            if (string.IsNullOrWhiteSpace(_bindingItemsSourcePath) == false)
            {
                _itemsSourceBindingData ??= _bindingItemsSourcePath.ToPropertyBindingData();
                _itemsSource = objectProvider
                    .RentReadOnlyProperty<ObservableCollection<string>>(context, _itemsSourceBindingData);
                _itemsSource.Value.CollectionChanged += OnItemsCollectionChanged;
                _dropdown.options = new List<TMP_Dropdown.OptionData>(_itemsSource.Value.Select(value => new TMP_Dropdown.OptionData(value)));
            }
            
            if (string.IsNullOrWhiteSpace(_bindingSelectedItemPath) == false)
            {
                _selectedItemBindingData ??= _bindingSelectedItemPath.ToPropertyBindingData();
                _selectedItemProperty = objectProvider.RentProperty<string>(context, _selectedItemBindingData);
                _selectedItemProperty.ValueChanged += OnPropertySelectedItemChanged;
                
                var foundIndex = _dropdown.options.FindIndex(option => option.text == _selectedItemProperty.Value);
                if (foundIndex != -1)
                {
                    UpdateControlValue(foundIndex);
                }
                
                _dropdown.onValueChanged.AddListener(OnControlValueChanged);
                _selectedItemProperty.Value = _dropdown.options.Count > 0 ? _dropdown.options[0].text : default;
            }
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (string newItem in e.NewItems)
                {
                    _dropdown.options.Add(new TMP_Dropdown.OptionData(newItem));
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (string oldItem in e.OldItems)
                {
                    _dropdown.options.Remove(new TMP_Dropdown.OptionData(oldItem));
                }
            }
            
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _dropdown.options.Clear();
            }
        }
        
        public virtual void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_itemsSource != null)
            {
                _itemsSource.Value.CollectionChanged -= OnItemsCollectionChanged;
                objectProvider.ReturnReadOnlyProperty(_itemsSource);
                _itemsSource = null;
                _dropdown.options = new List<TMP_Dropdown.OptionData>();
            }
            
            if (_selectedItemProperty != null)
            {
                _selectedItemProperty.ValueChanged -= OnPropertySelectedItemChanged;
                objectProvider.ReturnProperty(_selectedItemProperty);
                _selectedItemProperty = null;
                _dropdown.onValueChanged.RemoveListener(OnControlValueChanged);
            }
            
            UpdateControlValue(default);
        }

        protected virtual void OnControlValueChanged(int index)
        {
            _selectedItemProperty.Value = _dropdown.options[index].text;
        }

        private void OnPropertySelectedItemChanged(object sender, string newValue)
        {
            var foundIndex = _dropdown.options.FindIndex(option => option.text == newValue);
            if (foundIndex == -1)
            {
                return;
            }
            
            UpdateControlValue(foundIndex);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void UpdateControlValue(int newValue)
        {
            _dropdown.SetValueWithoutNotify(newValue);
        }
    }
}

#endif