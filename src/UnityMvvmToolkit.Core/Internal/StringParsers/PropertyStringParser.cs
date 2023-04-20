using System;

namespace UnityMvvmToolkit.Core.Internal.StringParsers
{
    internal sealed class PropertyStringParser : BindingStringParser
    {
        private const string ConverterOpen = "Converter={";

        public PropertyBindingData GetPropertyData(ReadOnlyMemory<char> propertyBindingPath)
        {
            var propertyData = new PropertyBindingData();
            var isShortFormat = IsShortFormat(propertyBindingPath);

            foreach (var line in Split(propertyBindingPath))
            {
                AssureLineIsNotEmpty(line.Data);

                if (isShortFormat)
                {
                    propertyData.SetValueByIndex(line.Index, propertyBindingPath.Slice(line.Start, line.Length));
                    continue;
                }

                if (IsBindingOption(ConverterOpen, line, propertyBindingPath, out var converterName))
                {
                    propertyData.ConverterName = converterName.ToString();
                    continue;
                }

                propertyData.PropertyName = propertyBindingPath.Slice(line.Start, line.Length).ToString();
            }

            return propertyData;
        }
    }
}