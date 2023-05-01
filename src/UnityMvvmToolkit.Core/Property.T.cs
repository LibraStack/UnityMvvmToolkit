using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core
{
    public sealed class Property<T> : IProperty<T>
    {
        private T _value;

        public Property() : this(default)
        {
        }

        public Property(T value)
        {
            _value = value;
        }

        public T Value
        {
            get => _value;
            set => TrySetValue(value);
        }

        public event EventHandler<T> ValueChanged;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetValue(T value)
        {
            if (EqualityComparer<T>.Default.Equals(_value, value))
            {
                return false;
            }

            SetValue(value);
            return true;
        }

        void IProperty<T>.ForceSetValue(T value)
        {
            SetValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetValue(T value)
        {
            _value = value;
            ValueChanged?.Invoke(this, value);
        }

        public static implicit operator Property<T>(T value)
        {
            return new Property<T>(value);
        }
    }
}