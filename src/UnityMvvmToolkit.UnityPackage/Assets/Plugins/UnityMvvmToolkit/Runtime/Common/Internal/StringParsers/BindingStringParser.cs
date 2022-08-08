using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Internal.Structs;

namespace UnityMvvmToolkit.Common.Internal.StringParsers
{
    internal abstract class BindingStringParser
    {
        private const char Comma = ',';
        private const char OptionClose = '}';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static LineSplitEnumerator Split(ReadOnlyMemory<char> bindingStringData, bool trim = true)
        {
            return bindingStringData.Split(Comma, trim);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static bool IsBindingOption(ReadOnlySpan<char> optionOpen, LineSplitData line,
            ReadOnlyMemory<char> bindingStringData, out ReadOnlyMemory<char> optionValue)
        {
            if (line.Data.Contains(optionOpen, out var startIndex) &&
                line.Data.Contains(OptionClose, out var closeIndex))
            {
                var start = line.Start + startIndex + optionOpen.Length;
                var length = closeIndex - (startIndex + optionOpen.Length);

                optionValue = bindingStringData.Slice(start, length);
                return true;
            }

            optionValue = default;
            return false;
        }
    }
}