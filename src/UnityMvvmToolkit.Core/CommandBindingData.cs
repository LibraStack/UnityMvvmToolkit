using System;

namespace UnityMvvmToolkit.Core
{
    public sealed class CommandBindingData : BindingData
    {
        public CommandBindingData(int elementId)
        {
            ElementId = elementId;
        }

        public int ElementId { get; }
        public string ParameterValue { get; set; }

        public override void SetValueByIndex(int index, ReadOnlyMemory<char> value)
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
                    ConverterName = value.ToString();
                    break;
                default: throw new IndexOutOfRangeException(nameof(index));
            }
        }
    }
}