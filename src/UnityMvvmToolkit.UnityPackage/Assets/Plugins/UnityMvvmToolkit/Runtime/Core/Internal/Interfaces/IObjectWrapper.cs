namespace UnityMvvmToolkit.Core.Internal.Interfaces
{
    internal interface IObjectWrapper
    {
        int ConverterId { get; }

        void Reset();
    }
}