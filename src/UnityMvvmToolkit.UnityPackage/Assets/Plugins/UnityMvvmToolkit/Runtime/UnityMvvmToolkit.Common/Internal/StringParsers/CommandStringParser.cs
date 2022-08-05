using System;
using UnityMvvmToolkit.Common.Extensions;
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

            foreach (var line in Split(commandStringData))
            {
                if (line.Data.IsEmptyOrWhiteSpace())
                {
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

                if (commandData.IsReady)
                {
                    break;
                }
            }

            return commandData;
        }
    }
}