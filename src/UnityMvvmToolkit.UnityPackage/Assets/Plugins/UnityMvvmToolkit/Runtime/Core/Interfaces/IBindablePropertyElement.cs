using System.Collections.Generic;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IBindablePropertyElement : IBindableElement
    {
        IEnumerable<string> BindableProperties { get; }

        void UpdateValues();
    }
}