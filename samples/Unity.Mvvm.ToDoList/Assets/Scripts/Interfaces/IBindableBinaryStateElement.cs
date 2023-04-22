using UnityMvvmToolkit.Core.Interfaces;

namespace Interfaces
{
    public interface IBindableBinaryStateElement : IBindableElement
    {
        string BindingStatePath { get; }

        void Activate();
        void Deactivate();
    }
}