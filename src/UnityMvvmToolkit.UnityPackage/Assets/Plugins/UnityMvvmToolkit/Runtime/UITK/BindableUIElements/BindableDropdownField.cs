using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.UITK.BindableUIElements
{
    public partial class BindableDropdownField : DropdownField, IBindableElement
    {
        private IProperty<string> _valueProperty;
        private IReadOnlyProperty<ObservableCollection<string>> _itemsSource;

        private PropertyBindingData _propertyBindingData;
        private PropertyBindingData _itemsSourceBindingData;
        
        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            if (string.IsNullOrWhiteSpace(BindingChoicesPath))
            {
                return;
            }

            _itemsSourceBindingData ??= BindingChoicesPath.ToPropertyBindingData();
            _propertyBindingData ??= BindingValuePath.ToPropertyBindingData();

            _itemsSource = objectProvider
                .RentReadOnlyProperty<ObservableCollection<string>>(context, _itemsSourceBindingData);
            _itemsSource.Value.CollectionChanged += OnItemsCollectionChanged;
            
            _valueProperty = objectProvider.RentProperty<string>(context, _propertyBindingData);
            _valueProperty.ValueChanged += OnPropertyValueChanged;

            UpdateControlValue(_valueProperty.Value);
            this.RegisterValueChangedCallback(OnControlValueChanged);

            choices = new List<string>(_itemsSource.Value);
            _valueProperty.Value = choices[0];
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
                        choices.Add(newValue);
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
                        choices.Add(oldValue);
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
                        int indexReplacedValue = choices.FindIndex(s => s == replacedValue);

                        if (indexReplacedValue != -1)
                        {
                            choices[indexReplacedValue] = replacingValue;
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
                        choices.Remove(oldItemMoved);
                        choices.Insert(e.NewStartingIndex, oldItemMoved);
                    }
                    break;
                
                case NotifyCollectionChangedAction.Reset:
                    choices.Clear();
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
            choices = new List<string>();

            objectProvider.ReturnProperty(_valueProperty);
            objectProvider.ReturnReadOnlyProperty(_itemsSource);
            
            _valueProperty = null;
            _itemsSource = null;

            this.UnregisterValueChangedCallback(OnControlValueChanged);
            UpdateControlValue(default);
        }

        protected virtual void OnControlValueChanged(ChangeEvent<string> e)
        {
            _valueProperty.Value = e.newValue;
        }

        private void OnPropertyValueChanged(object sender, string newValue)
        {
            UpdateControlValue(newValue);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void UpdateControlValue(string newValue)
        {
            SetValueWithoutNotify(newValue);
        }
    }
}