using System;
using UnityMvvmToolkit.Core.Interfaces;

#pragma warning disable CS0067

namespace UnityMvvmToolkit.Core
{
    public sealed class ReadOnlyProperty<T> : IReadOnlyProperty<T>
    {
        public ReadOnlyProperty(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public event EventHandler<T> ValueChanged;

        public static implicit operator ReadOnlyProperty<T>(T value)
        {
            return new ReadOnlyProperty<T>(value);
        }
    }
}