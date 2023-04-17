using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.Interfaces
{
    internal interface ICommandWrapperWithParameter : ICommandWrapper
    {
        ICommandWrapper RegisterParameter(int elementId, string parameter);
        ICommandWrapper UnregisterParameter(int elementId);
    }
}