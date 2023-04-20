namespace UnityMvvmToolkit.Core.Internal.Interfaces
{
    internal interface IObjectWrapper<out T> : IObjectWrapper
    {
        T SetConverterId(int converterId);
    }
}