using System;
using UnityMvvmToolkit.Core.Interfaces;

#pragma warning disable CS0067

namespace UnityMvvmToolkit.Core
{
    public sealed class ReadOnlyProperty<TType> : IReadOnlyProperty<TType>
    {
        public ReadOnlyProperty(TType value)
        {
            Value = value;
        }

        public TType Value { get; }

        public event EventHandler<TType> ValueChanged;

        public static implicit operator ReadOnlyProperty<TType>(TType value)
        {
            return new ReadOnlyProperty<TType>(value);
        }
    }
}