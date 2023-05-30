using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IBindingContextProvider
    {
        bool IsValid { get; }
        IBindingContext BindingContext { get;}
    }
}