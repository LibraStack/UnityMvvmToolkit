using System;

namespace UnityMvvmToolkit.Core.Internal.StringParsers
{
    internal sealed class CommandStringParser : BindingStringParser
    {
        private const string ParameterOpen = "Parameter={";
        private const string ConverterOpen = "Converter={";

        public CommandBindingData GetCommandData(int elementId, ReadOnlyMemory<char> commandBindingPath)
        {
            var commandData = new CommandBindingData(elementId);
            var isShortFormat = IsShortFormat(commandBindingPath);

            foreach (var line in Split(commandBindingPath))
            {
                AssureLineIsNotEmpty(line.Data);

                if (isShortFormat)
                {
                    commandData.SetValueByIndex(line.Index, commandBindingPath.Slice(line.Start, line.Length));
                    continue;
                }

                if (IsBindingOption(ParameterOpen, line, commandBindingPath, out var parameterValue))
                {
                    commandData.ParameterValue = parameterValue.ToString();
                    continue;
                }

                if (IsBindingOption(ConverterOpen, line, commandBindingPath, out var converterName))
                {
                    commandData.ConverterName = converterName.ToString();
                    continue;
                }

                commandData.PropertyName = commandBindingPath.Slice(line.Start, line.Length).ToString();
            }

            return commandData;
        }
    }
}