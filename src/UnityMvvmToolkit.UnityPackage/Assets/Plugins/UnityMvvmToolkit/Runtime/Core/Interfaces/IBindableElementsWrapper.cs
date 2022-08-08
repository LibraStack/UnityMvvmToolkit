namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IBindableElementsWrapper
    {
        IBindableElement Wrap(IBindableUIElement bindableUiElement, IObjectProvider objectProvider);
    }
}