using System;

namespace UnityMvvmToolkit.Common.Internal.Structs
{
    internal ref struct PropertyData
    {
        public ReadOnlyMemory<char> PropertyName { get; set; }
        public ReadOnlyMemory<char> ConverterName { get; set; }

        public bool IsReady => PropertyName.IsEmpty == false &&
                               ConverterName.IsEmpty == false;
    }
}