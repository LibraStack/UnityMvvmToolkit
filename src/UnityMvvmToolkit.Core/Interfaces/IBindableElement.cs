namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IBindableElement
    {
        void SetBindingContext(IBindingContext context, IObjectProvider objectProvider);
        void ResetBindingContext(IObjectProvider objectProvider);
    }
}