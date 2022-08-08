using System.Collections.Generic;

namespace UnityMvvmToolkit.Common.Interfaces
{
    public interface IBindablePropertyElement : IBindableElement
    {
        IEnumerable<string> BindableProperties { get; }

        void UpdateValues();
    }
}