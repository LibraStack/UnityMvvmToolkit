namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface ICommand<in T> : IBaseCommand
    {
        void Execute(T parameter);
    }
}