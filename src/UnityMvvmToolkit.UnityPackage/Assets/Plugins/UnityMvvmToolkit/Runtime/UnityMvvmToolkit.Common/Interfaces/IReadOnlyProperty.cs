namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IReadOnlyProperty<out TValueType>
    {
        TValueType Value { get; }
    }
}