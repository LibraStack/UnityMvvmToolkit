using System;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Internal.Structs;

namespace UnityMvvmToolkit.Common.Internal
{
    internal class BindingStringParser
    {
        private const char Comma = ',';
        private const string ConverterOpen = "Converter={";
        private const char ConverterClose = '}';

        internal BindingData GetBindingData(ReadOnlyMemory<char> bindingStringData)
        {
            var bindingData = new BindingData();

            foreach (var line in bindingStringData.Split(Comma, true))
            {
                if (line.IsEmptyOrWhiteSpace)
                {
                    continue;
                }

                var converterStartIndex = line.Data.IndexOf(ConverterOpen);
                if (converterStartIndex == -1)
                {
                    bindingData.PropertyName = bindingStringData.Slice(line.Start, line.Length);
                }
                else
                {
                    var converterCloseIndex = line.Data.IndexOf(ConverterClose);
                    if (converterCloseIndex == -1)
                    {
                        continue;
                    }

                    var start = line.Start + converterStartIndex + ConverterOpen.Length;
                    var length = converterCloseIndex - (converterStartIndex + ConverterOpen.Length);

                    bindingData.ConverterName = bindingStringData.Slice(start, length);
                }

                if (bindingData.IsReady)
                {
                    break;
                }
            }

            return bindingData;
        }
    }
}