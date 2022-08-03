using System.Collections.Generic;

namespace UnityMvvmToolkit.Common.Interfaces
{
    // TODO: Come up with a better name.
    public interface IBindableVisualElement : IBindableElement
    {
        IEnumerable<string> BindableProperties { get; }

        void UpdateValues();
    }
}