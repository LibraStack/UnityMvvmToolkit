using System;

namespace UnityMvvmToolkit.Core
{
    public sealed class PropertyBindingData : BindingData
    {
        public override void SetValueByIndex(int index, ReadOnlyMemory<char> value)
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