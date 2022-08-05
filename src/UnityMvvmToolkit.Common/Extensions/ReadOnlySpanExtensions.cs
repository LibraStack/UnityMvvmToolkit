using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Common.Extensions
{
    public static class ReadOnlySpanExtensions
    {
        private static readonly CultureInfo CommaCulture = new("en")
        {
            NumberFormat = { NumberDecimalSeparator = "," }
        };

        private static readonly CultureInfo PointCulture = new("en")
        {
            NumberFormat = { NumberDecimalSeparator = "." }
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmptyOrWhiteSpace(this ReadOnlySpan<char> span)
        {
            return span.IsEmpty || span.IsWhiteSpace();
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse(this ReadOnlySpan<char> span, out float result)
        {
            return float.TryParse(span, NumberStyles.Any, CommaCulture, out result) ||
                   float.TryParse(span, NumberStyles.Any, PointCulture, out result);
        }
    }
}