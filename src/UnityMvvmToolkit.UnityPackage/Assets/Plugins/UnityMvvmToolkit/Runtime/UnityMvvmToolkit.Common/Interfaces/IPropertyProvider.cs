namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IPropertyProvider
    {
        IProperty<TValueType> GetProperty<TValueType>(string propertyName);
        IReadOnlyProperty<TValueType> GetReadOnlyProperty<TValueType>(string propertyName);
    }
}