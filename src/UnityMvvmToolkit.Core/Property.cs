using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core
{
    public sealed class Property<TType> : IProperty<TType>
    {
        private TType _value;

        public Property() : this(default)
        {
        }

        public Property(TType value)
        {
            _value = value;
        }

        public TType Value
        {
            get => _value;
            set => TrySetValue(value);
        }

        public event EventHandler<TType> ValueChanged;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetValue(TType value)
        {
            if (EqualityComparer<TType>.Default.Equals(_value, value))
            {
                return false;
            }

            SetValue(value);
            return true;
        }

        void IProperty<TType>.ForceSetValue(TType value)
        {
            SetValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetValue(TType value)
        {
            _value = value;
            ValueChanged?.Invoke(this, value);
        }

        public static implicit operator Property<TType>(TType value)
        {
            return new Property<TType>(value);
        }
    }
}