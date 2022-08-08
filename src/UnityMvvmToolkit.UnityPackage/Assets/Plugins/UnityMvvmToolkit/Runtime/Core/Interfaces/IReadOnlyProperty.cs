namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IReadOnlyProperty<out TValueType>
    {
        TValueType Value { get; }
    }
}