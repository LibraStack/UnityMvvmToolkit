using System;
using UnityMvvmToolkit.Common.Internal.Structs;

namespace UnityMvvmToolkit.Common.Internal.StringParsers
{
    internal class CommandStringParser : BindingStringParser
    {
        private const string ParameterOpen = "Parameter={";
        private const string ConverterOpen = "Converter={";

        public CommandData GetCommandData(ReadOnlyMemory<char> commandStringData)
        {
            var commandData = new CommandData();
            var isShortFormat = IsShortFormat(commandStringData);

            foreach (var line in Split(commandStringData))
            {
                AssureLineIsNotEmpty(line.Data);

                if (isShortFormat)
                {
                    commandData.SetValueByIndex(line.Index, commandStringData.Slice(line.Start, line.Length));
                    continue;
                }

                if (IsBindingOption(ParameterOpen, line, commandStringData, out var parameterValue))
                {
                    commandData.ParameterValue = parameterValue;
                    continue;
                }

                if (IsBindingOption(ConverterOpen, line, commandStringData, out var converterName))
                {
                    commandData.ParameterConverterName = converterName;
                    continue;
                }

                commandData.PropertyName = commandStringData.Slice(line.Start, line.Length);
            }

            return commandData;
        }
    }
}