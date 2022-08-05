namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IBindableVisualElementsCreator
    {
        IBindableElement Create(IBindableUIElement bindableUiElement, IObjectProvider objectProvider);
    }
}