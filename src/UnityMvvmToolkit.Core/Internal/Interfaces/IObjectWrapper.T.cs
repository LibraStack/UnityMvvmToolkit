namespace UnityMvvmToolkit.Core.Internal.Interfaces
{
    internal interface IObjectWrapper<out T> : IObjectWrapper
    {
        int ConverterId { get; }

        T SetConverterId(int converterId);

        void Reset();
    }
}