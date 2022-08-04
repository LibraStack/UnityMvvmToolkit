using System;

namespace UnityMvvmToolkit.Common.Internal.Structs
{
    internal ref struct BindingData
    {
        public bool IsReady => PropertyName.IsEmpty == false && ConverterName.IsEmpty == false;
        public ReadOnlyMemory<char> PropertyName { get; set; }
        public ReadOnlyMemory<char> ConverterName { get; set; }
    }
}