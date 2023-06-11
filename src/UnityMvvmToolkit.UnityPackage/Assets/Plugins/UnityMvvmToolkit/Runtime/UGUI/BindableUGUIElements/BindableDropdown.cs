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
        [SerializeField] private string _bindingValuePath;
        [SerializeField] private string _bindingChoicesPath;

        private IProperty<string> _valueProperty;
        private IReadOnlyProperty<ObservableCollection<string>> _itemsSource;

        private PropertyBindingData _propertyBindingData;
        private PropertyBindingData _itemsSourceBindingData;
        
        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            if (string.IsNullOrWhiteSpace(_bindingChoicesPath))
            {
                return;
            }

            _itemsSourceBindingData ??= _bindingChoicesPath.ToPropertyBindingData();
            _propertyBindingData ??= _bindingValuePath.ToPropertyBindingData();

            _itemsSource = objectProvider
                .RentReadOnlyProperty<ObservableCollection<string>>(context, _itemsSourceBindingData);
            _itemsSource.Value.CollectionChanged += OnItemsCollectionChanged;
            
            _valueProperty = objectProvider.RentProperty<string>(context, _propertyBindingData);
            _valueProperty.ValueChanged += OnPropertyValueChanged;

            UpdateControlValue(_dropdown.options.FindIndex(option => option.text == _valueProperty.Value));
            _dropdown.onValueChanged.AddListener(OnControlValueChanged);

            _dropdown.options = new List<TMP_Dropdown.OptionData>(_itemsSource.Value.Select(value => new TMP_Dropdown.OptionData(value)));
            _valueProperty.Value = _dropdown.options[0].text;
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    
                    if (e.NewItems.Count != 1)
                    {
                        throw new NotSupportedException("RangeActionsNotSupported");
                    }
                    
                    if (e.NewItems?[0] is string newValue)
                    {
                        _dropdown.options.Add(new TMP_Dropdown.OptionData(newValue));
                    }
                    break;
                
                case NotifyCollectionChangedAction.Remove:

                    if (e.OldItems.Count != 1)
                    {
                        throw new NotSupportedException("RangeActionsNotSupported");
                    }

                    if (e.OldStartingIndex < 0)
                    {
                        throw new InvalidOperationException("RemovedItemNotFound");
                    }
                    
                    
                    if (e.OldItems?[0] is string oldValue)
                    {
                        _dropdown.options.Remove(new TMP_Dropdown.OptionData(oldValue));
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
            if (_valueProperty == null || _itemsSource == null)
            {
                return;
            }

            _valueProperty.ValueChanged -= OnPropertyValueChanged;
            _itemsSource.Value.CollectionChanged -= OnItemsCollectionChanged;
            _dropdown.options = new List<TMP_Dropdown.OptionData>();
            
            objectProvider.ReturnProperty(_valueProperty);
            objectProvider.ReturnReadOnlyProperty(_itemsSource);
            
            _valueProperty = null;
            _itemsSource = null;

            _dropdown.onValueChanged.RemoveListener(OnControlValueChanged);
            UpdateControlValue(default);
        }

        protected virtual void OnControlValueChanged(int index)
        {
            _valueProperty.Value = _dropdown.options[index].text;
        }

        private void OnPropertyValueChanged(object sender, string newValue)
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