using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Common.Extensions
{
    public static class SpanExtensions
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
        public static bool TryParse(this ReadOnlySpan<char> str, out int result)
        {
            return int.TryParse(str, out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse(this ReadOnlySpan<char> str, out float result)
        {
            return float.TryParse(str, NumberStyles.Any, CommaCulture, out result) ||
                   float.TryParse(str, NumberStyles.Any, PointCulture, out result);
        }
    }
}