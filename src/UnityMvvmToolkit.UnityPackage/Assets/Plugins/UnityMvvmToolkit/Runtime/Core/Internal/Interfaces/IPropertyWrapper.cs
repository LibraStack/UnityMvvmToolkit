namespace UnityMvvmToolkit.Core.Internal.Interfaces
{
    internal interface IPropertyWrapper
    {
        int ConverterId { get; }

        IPropertyWrapper SetConverterId(int converterId);
        IPropertyWrapper SetProperty(object property);
        void Reset();
    }
}