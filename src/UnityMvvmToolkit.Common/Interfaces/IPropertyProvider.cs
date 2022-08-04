using System;

namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IPropertyProvider
    {
        TCommand GetCommand<TCommand>(string propertyName) where TCommand : IBaseCommand;
        IProperty<TValueType> GetProperty<TValueType>(string propertyName, ReadOnlyMemory<char> converterName);
        IReadOnlyProperty<TValueType> GetReadOnlyProperty<TValueType>(string propertyName, ReadOnlyMemory<char> converterName);
    }
}