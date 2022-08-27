using UnityMvvmToolkit.Core.Interfaces;

namespace Interfaces
{
    public interface IBindableBinaryStateElement : IBindableUIElement
    {
        string BindingStatePath { get; }

        void Activate();
        void Deactivate();
    }
}