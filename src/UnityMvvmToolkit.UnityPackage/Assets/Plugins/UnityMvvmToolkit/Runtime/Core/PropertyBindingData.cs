using System;

namespace UnityMvvmToolkit.Core
{
    public class PropertyBindingData
    {
        public string PropertyName { get; set; }
        public string ConverterName { get; set; }

        public void SetValueByIndex(int index, ReadOnlyMemory<char> value)
        {
            switch (index)
            {
                case 0:
                    PropertyName = value.ToString();
                    break;
                case 1:
                    ConverterName = value.ToString();
                    break;
                default: throw new IndexOutOfRangeException(nameof(index));
            }
        }
    }
}