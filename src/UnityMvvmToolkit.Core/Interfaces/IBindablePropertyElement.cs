using System.Collections.Generic;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IBindablePropertyElement : IBindableElement
    {
        IReadOnlyCollection<string> BindableProperties { get; }

        void UpdateValues();
    }
}