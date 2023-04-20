using System;

namespace UnityMvvmToolkit.Core
{
    public abstract class BindingData
    {
        public string PropertyName { get; set; }
        public string ConverterName { get; set; }

        public abstract void SetValueByIndex(int index, ReadOnlyMemory<char> value);
    }
}