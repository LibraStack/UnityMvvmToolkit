using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.Interfaces
{
    internal interface ICommandWrapper : IObjectWrapper<ICommandWrapper>, IBaseCommand
    {
        int CommandId { get; }

        ICommandWrapper SetCommand(int commandId, IBaseCommand command);

        ICommandWrapper RegisterParameter(int elementId, string parameter);
        int UnregisterParameter(int elementId);
    }
}