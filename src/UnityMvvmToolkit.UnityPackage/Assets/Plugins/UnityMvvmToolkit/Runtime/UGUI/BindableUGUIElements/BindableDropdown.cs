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
                UpdateControlValue(_dropdown.options.FindIndex(option => option.text == _selectedItemProperty.Value));
                _dropdown.onValueChanged.AddListener(OnControlValueChanged);
                _selectedItemProperty.Value = _dropdown.options.Count > 0 ? _dropdown.options[0].text : default;
            }
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    
                    foreach (string newItem in e.NewItems)
                    {
                        _dropdown.options.Add(new TMP_Dropdown.OptionData(newItem));
                    }

                    break;
                
                case NotifyCollectionChangedAction.Remove:

                    if (e.OldStartingIndex < 0)
                    {
                        throw new InvalidOperationException("RemovedItemNotFound");
                    }
                    
                    foreach (string oldItem in e.OldItems)
                    {
                        _dropdown.options.Remove(new TMP_Dropdown.OptionData(oldItem));
                    }

                    break;
                
                case NotifyCollectionChangedAction.Replace:
                    
                    if (e.NewItems.Count != 1 || e.OldItems.Count != 1)
                    {
                        throw new NotSupportedException("RangeActionsNotSupported");
                    }
                    
                    if (e.NewItems?[0] is string replacingValue &&
                        e.OldItems?[0] is string replacedValue)
                    {
                        int indexReplacedValue =  _dropdown.options.FindIndex(s => s.text == replacedValue);

                        if (indexReplacedValue != -1)
                        {
                            _dropdown.options[indexReplacedValue] = new TMP_Dropdown.OptionData(replacingValue);
                        }
                    }
                    break;
                
                case NotifyCollectionChangedAction.Move:
                    
                    if (e.NewItems.Count != 1)
                    {
                        throw new NotSupportedException("RangeActionsNotSupported");
                    }
                    if (e.NewStartingIndex < 0)
                    {
                        throw new InvalidOperationException("CannotMoveToUnknownPosition");
                    }
                    
                    if (e.OldItems?[0] is string oldItemMoved)
                    {
                        _dropdown.options.Remove(new TMP_Dropdown.OptionData(oldItemMoved));
                        _dropdown.options.Insert(e.NewStartingIndex, new TMP_Dropdown.OptionData(oldItemMoved));
                    }
                    break;
                
                case NotifyCollectionChangedAction.Reset:
                    _dropdown.options.Clear();
                    break;
                
                default:
                {
                    throw new NotSupportedException("UnexpectedCollectionChangeAction");
                }
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
            UpdateControlValue(_dropdown.options.FindIndex(option => option.text == newValue));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void UpdateControlValue(int newValue)
        {
            _dropdown.SetValueWithoutNotify(newValue);
        }
    }
}

#endif