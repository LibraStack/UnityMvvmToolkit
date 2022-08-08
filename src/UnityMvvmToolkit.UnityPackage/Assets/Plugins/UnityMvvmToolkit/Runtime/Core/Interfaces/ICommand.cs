namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface ICommand : IBaseCommand
    {
        void Execute();
    }

    public interface ICommand<in T> : IBaseCommand
    {
        void Execute(T parameter);
    }
}