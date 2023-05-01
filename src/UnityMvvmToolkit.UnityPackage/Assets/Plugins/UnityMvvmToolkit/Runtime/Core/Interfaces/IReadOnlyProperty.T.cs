using System;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IReadOnlyProperty<T> : IBaseProperty
    {
        T Value { get; }

        event EventHandler<T> ValueChanged;
    }
}