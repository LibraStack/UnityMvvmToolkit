namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IInitializable
    {
        bool CanInitialize { get; }
        void Initialize();
    }
}