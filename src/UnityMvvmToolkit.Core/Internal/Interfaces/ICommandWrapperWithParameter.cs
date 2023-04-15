using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.Interfaces
{
    internal interface ICommandWrapperWithParameter : ICommandWrapper
    {
        void SetParameter(int elementId, string parameter);
    }
}