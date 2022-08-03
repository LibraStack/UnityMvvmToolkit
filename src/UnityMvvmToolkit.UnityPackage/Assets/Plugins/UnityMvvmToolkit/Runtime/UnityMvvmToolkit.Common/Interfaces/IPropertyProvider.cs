namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IPropertyProvider
    {
        TCommand GetCommand<TCommand>(string propertyName) where TCommand : IBaseCommand;
        IProperty<TValueType> GetProperty<TValueType>(string propertyName);
        IReadOnlyProperty<TValueType> GetReadOnlyProperty<TValueType>(string propertyName);
    }
}