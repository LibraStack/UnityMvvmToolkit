using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Extensions
{
    public static class ReadOnlySpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this ReadOnlySpan<char> span, char value, out int index)
        {
            index = span.IndexOf(value);
            return index != -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this ReadOnlySpan<char> span, ReadOnlySpan<char> value, out int index)
        {
            index = span.IndexOf(value);
            return index != -1;
        }
    }
}