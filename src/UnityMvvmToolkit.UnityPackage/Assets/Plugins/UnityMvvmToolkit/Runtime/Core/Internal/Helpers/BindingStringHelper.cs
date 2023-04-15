using System;
using UnityMvvmToolkit.Core.Internal.StringParsers;

namespace UnityMvvmToolkit.Core.Internal.Helpers
{
    internal static class BindingStringHelper
    {
        private static readonly CommandStringParser CommandStringParser;
        private static readonly PropertyStringParser PropertyStringParser;

        static BindingStringHelper()
        {
            CommandStringParser = new CommandStringParser();
            PropertyStringParser = new PropertyStringParser();
        }

        internal static CommandBindingData GetCommandBindingData(int elementId, string bindingString)
        {
            return CommandStringParser.GetCommandData(elementId, bindingString.AsMemory());
        }

        internal static PropertyBindingData GetPropertyBindingData(string bindingString)
        {
            return PropertyStringParser.GetPropertyData(bindingString.AsMemory());
        }
    }
}