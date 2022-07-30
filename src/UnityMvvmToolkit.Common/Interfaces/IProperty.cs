namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IProperty<TValueType>
    {
        TValueType Value { get; set; }
    }
}