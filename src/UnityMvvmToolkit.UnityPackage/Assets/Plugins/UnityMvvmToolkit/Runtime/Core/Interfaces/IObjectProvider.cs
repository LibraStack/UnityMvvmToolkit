using System;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IObjectProvider
    {
        IProperty<TValueType> GetProperty<TValueType>(string propertyName, ReadOnlyMemory<char> converterName);
        IReadOnlyProperty<TValueType> GetReadOnlyProperty<TValueType>(string propertyName,
            ReadOnlyMemory<char> converterName);

        TCommand GetCommand<TCommand>(string propertyName) where TCommand : IBaseCommand;
        ICommandWrapper GetCommandWrapper(string propertyName, ReadOnlyMemory<char> parameterConverterName);
    }
}