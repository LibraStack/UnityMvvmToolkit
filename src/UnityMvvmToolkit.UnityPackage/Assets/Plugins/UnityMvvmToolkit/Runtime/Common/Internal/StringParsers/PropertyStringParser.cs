using System;
using UnityMvvmToolkit.Common.Internal.Structs;

namespace UnityMvvmToolkit.Common.Internal.StringParsers
{
    internal class PropertyStringParser : BindingStringParser
    {
        private const string ConverterOpen = "Converter={";

        public PropertyData GetPropertyData(ReadOnlyMemory<char> propertyStringData)
        {
            var propertyData = new PropertyData();
            var isShortFormat = IsShortFormat(propertyStringData);

            foreach (var line in Split(propertyStringData))
            {
                AssureLineIsNotEmpty(line.Data);

                if (isShortFormat)
                {
                    propertyData.SetValueByIndex(line.Index, propertyStringData.Slice(line.Start, line.Length));
                    continue;
                }

                if (IsBindingOption(ConverterOpen, line, propertyStringData, out var converterName))
                {
                    propertyData.ConverterName = converterName;
                    continue;
                }

                propertyData.PropertyName = propertyStringData.Slice(line.Start, line.Length);
            }

            return propertyData;
        }
    }
}