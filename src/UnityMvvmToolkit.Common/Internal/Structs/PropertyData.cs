using System;

namespace UnityMvvmToolkit.Common.Internal.Structs
{
    internal ref struct PropertyData
    {
        public ReadOnlyMemory<char> PropertyName { get; set; }
        public ReadOnlyMemory<char> ConverterName { get; set; }

        public void SetValueByIndex(int index, ReadOnlyMemory<char> value)
        {
            switch (index)
            {
                case 0:
                    PropertyName = value;
                    break;
                case 1:
                    ConverterName = value;
                    break;
                default: throw new IndexOutOfRangeException(nameof(index));
            }
        }
    }
}