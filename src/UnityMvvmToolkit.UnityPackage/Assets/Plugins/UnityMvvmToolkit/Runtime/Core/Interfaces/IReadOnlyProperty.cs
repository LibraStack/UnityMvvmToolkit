using System;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IReadOnlyProperty<TType> : IBaseProperty
    {
        TType Value { get; }

        event EventHandler<TType> ValueChanged;
    }
}