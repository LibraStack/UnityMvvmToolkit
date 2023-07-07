using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.Interfaces
{
    internal interface IPropertyWrapper : IObjectWrapper<IPropertyWrapper>
    {
        IPropertyWrapper SetProperty(IBaseProperty property);
    }
}