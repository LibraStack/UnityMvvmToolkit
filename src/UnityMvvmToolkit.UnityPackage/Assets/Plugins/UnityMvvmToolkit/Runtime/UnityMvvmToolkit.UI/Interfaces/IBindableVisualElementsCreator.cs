using System.Reflection;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.UI.Interfaces
{
    public interface IBindableVisualElementsCreator
    {
        IBindableElement Create<TBindingContext>(TBindingContext bindingContext, IBindableUIElement bindableUIElement,
            PropertyInfo propertyInfo);
    }
}