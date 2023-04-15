using System;

namespace UnityMvvmToolkit.Core
{
    public class CommandBindingData
    {
        public CommandBindingData(int elementId)
        {
            ElementId = elementId;
        }

        public int ElementId { get; }
        public string PropertyName { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterConverterName { get; set; }

        public void SetValueByIndex(int index, ReadOnlyMemory<char> value)
        {
            switch (index)
            {
                case 0:
                    PropertyName = value.ToString();
                    break;
                case 1:
                    ParameterValue = value.ToString();
                    break;
                case 2:
                    ParameterConverterName = value.ToString();
                    break;
                default: throw new IndexOutOfRangeException(nameof(index));
            }
        }
    }
}