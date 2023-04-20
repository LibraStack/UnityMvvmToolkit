namespace UnityMvvmToolkit.Core.Internal.Interfaces
{
    internal interface IPropertyWrapper : IObjectWrapper<IPropertyWrapper>
    {
        IPropertyWrapper SetProperty(object property);
    }
}