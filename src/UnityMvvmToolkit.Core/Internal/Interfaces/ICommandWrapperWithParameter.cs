using System;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.Interfaces
{
    internal interface ICommandWrapperWithParameter : ICommandWrapper
    {
        void SetParameter(int elementId, ReadOnlyMemory<char> parameter);
    }
}