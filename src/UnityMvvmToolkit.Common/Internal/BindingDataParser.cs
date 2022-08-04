using System;
using UnityMvvmToolkit.Common.Extensions;

namespace UnityMvvmToolkit.Common.Internal
{
    internal class BindingDataParser
    {
        private const char Comma = ',';
        private const string ConverterOpen = "Converter={";
        private const char ConverterClose = '}';

        internal BindingData GetBindingData(ReadOnlyMemory<char> bindingStringData)
        {
            var bindingData = new BindingData();

            foreach (var lineData in bindingStringData.Split(Comma))
            {
                if (lineData.IsEmptyOrWhiteSpace)
                {
                    continue;
                }

                var resultLine = lineData.Trim();

                var converterStartIndex = resultLine.Data.IndexOf(ConverterOpen);
                if (converterStartIndex == -1)
                {
                    bindingData.PropertyName = bindingStringData.Slice(resultLine.Start, resultLine.Length);
                }
                else
                {
                    var converterCloseIndex = resultLine.Data.IndexOf(ConverterClose);
                    if (converterCloseIndex == -1)
                    {
                        continue;
                    }

                    var start = resultLine.Start + converterStartIndex + ConverterOpen.Length;
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