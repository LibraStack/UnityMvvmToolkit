using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface ICollectionItem : IBindingContext
    {
        int Id { get; }
    }
}