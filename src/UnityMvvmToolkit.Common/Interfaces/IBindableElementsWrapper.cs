namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IBindableElementsWrapper
    {
        IBindableElement Wrap(IBindableUIElement bindableUiElement, IObjectProvider objectProvider);
    }
}