namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface ICommand : IBaseCommand
    {
        void Execute();

        void IBaseCommand.Execute(int elementId)
        {
            Execute();
        }
    }
}