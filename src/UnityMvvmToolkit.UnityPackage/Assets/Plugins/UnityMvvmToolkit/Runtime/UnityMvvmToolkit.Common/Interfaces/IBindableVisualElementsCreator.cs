namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IBindableVisualElementsCreator
    {
        IBindableElement Create(IBindableUIElement bindableUiElement, IPropertyProvider propertyProvider);
    }
}