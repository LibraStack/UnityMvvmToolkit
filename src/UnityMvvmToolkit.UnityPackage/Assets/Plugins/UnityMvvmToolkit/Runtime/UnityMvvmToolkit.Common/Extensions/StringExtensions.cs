using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Internal.Structs;

namespace UnityMvvmToolkit.Common.Extensions
{
    public static class StringExtensions
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
        public static bool TryParse(this string str, out float result)
        {
            return float.TryParse(str, NumberStyles.Any, CommaCulture, out result) ||
                   float.TryParse(str, NumberStyles.Any, PointCulture, out result);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static LineSplitEnumerator Split(this ReadOnlyMemory<char> strMemory, char separator, bool trim = false)
        {
            return new LineSplitEnumerator(strMemory.Span, separator, trim);
        }
    }
}