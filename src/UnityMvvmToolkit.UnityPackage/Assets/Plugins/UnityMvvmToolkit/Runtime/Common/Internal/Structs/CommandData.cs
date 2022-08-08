using System;

namespace UnityMvvmToolkit.Common.Internal.Structs
{
    internal ref struct CommandData
    {
        public ReadOnlyMemory<char> PropertyName { get; set; }
        public ReadOnlyMemory<char> ParameterValue { get; set; }
        public ReadOnlyMemory<char> ParameterConverterName { get; set; }

        public bool IsReady => PropertyName.IsEmpty == false &&
                               ParameterValue.IsEmpty == false &&
                               ParameterConverterName.IsEmpty == false;
    }
}