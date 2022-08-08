using System;

namespace UnityMvvmToolkit.Common.Internal.Structs
{
    internal ref struct CommandData
    {
        public ReadOnlyMemory<char> PropertyName { get; set; }
        public ReadOnlyMemory<char> ParameterValue { get; set; }
        public ReadOnlyMemory<char> ParameterConverterName { get; set; }

        public void SetValueByIndex(int index, ReadOnlyMemory<char> value)
        {
            switch (index)
            {
                case 0:
                    PropertyName = value;
                    break;
                case 1:
                    ParameterValue = value;
                    break;
                case 2:
                    ParameterConverterName = value;
                    break;
                default: throw new IndexOutOfRangeException(nameof(index));
            }
        }
    }
}