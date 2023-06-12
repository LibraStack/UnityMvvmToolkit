using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
            if (string.IsNullOrWhiteSpace(BindingItemsSourcePath))
            {
                return;
            }

            _itemsSourceBindingData ??= BindingItemsSourcePath.ToPropertyBindingData();
            _selectedItemBindingData ??= BindingSelectedItemPath.ToPropertyBindingData();

            _itemsSource = objectProvider
                .RentReadOnlyProperty<ObservableCollection<string>>(context, _itemsSourceBindingData);
            _itemsSource.Value.CollectionChanged += OnItemsCollectionChanged;
            
            _selectedItemProperty = objectProvider.RentProperty<string>(context, _selectedItemBindingData);
            _selectedItemProperty.ValueChanged += OnSelectedItemValueChanged;

            UpdateControlValue(_selectedItemProperty.Value);
            this.RegisterValueChangedCallback(OnControlValueChanged);

            choices = new List<string>(_itemsSource.Value);
            _selectedItemProperty.Value = choices[0];
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
            if (_selectedItemProperty == null || _itemsSource == null)
            {
                return;
            }

            _selectedItemProperty.ValueChanged -= OnSelectedItemValueChanged;
            _itemsSource.Value.CollectionChanged -= OnItemsCollectionChanged;
            choices = new List<string>();

            objectProvider.ReturnProperty(_selectedItemProperty);
            objectProvider.ReturnReadOnlyProperty(_itemsSource);
            
            _selectedItemProperty = null;
            _itemsSource = null;

            this.UnregisterValueChangedCallback(OnControlValueChanged);
            UpdateControlValue(default);
        }

        protected virtual void OnControlValueChanged(ChangeEvent<string> e)
        {
            _selectedItemProperty.Value = e.newValue;
        }

        private void OnSelectedItemValueChanged(object sender, string newValue)
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