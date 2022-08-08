using System;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Internal.Structs;

namespace UnityMvvmToolkit.Common.Internal.StringParsers
{
    internal class PropertyStringParser : BindingStringParser
    {
        private const string ConverterOpen = "Converter={";

        public PropertyData GetPropertyData(ReadOnlyMemory<char> propertyStringData)
        {
            var bindingData = new PropertyData();

            foreach (var line in Split(propertyStringData))
            {
                if (line.Data.IsEmptyOrWhiteSpace())
                {
                    continue;
                }

                if (IsBindingOption(ConverterOpen, line, propertyStringData, out var converterName))
                {
                    bindingData.ConverterName = converterName;
                    continue;
                }

                bindingData.PropertyName = propertyStringData.Slice(line.Start, line.Length);

                if (bindingData.IsReady)
                {
                    break;
                }
            }

            return bindingData;
        }
    }
}