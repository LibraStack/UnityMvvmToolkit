using System.Reflection;

namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IBindableVisualElementsCreator
    {
        IBindableElement Create<TBindingContext>(TBindingContext bindingContext, IBindableUIElement bindableUiElement,
            PropertyInfo propertyInfo);
    }
}