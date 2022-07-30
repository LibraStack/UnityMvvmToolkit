using System.Reflection;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.UI.Interfaces
{
    public interface IBindableVisualElementsCreator<in TDataContext>
    {
        IBindableElement Create(IBindableUIElement bindableUIElement, TDataContext dataContext,
            PropertyInfo propertyInfo);
    }
}