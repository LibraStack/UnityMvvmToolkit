using System.Globalization;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Internal.Helpers;

namespace UnityMvvmToolkit.Core.Extensions
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

        public static CommandBindingData ToCommandBindingData(this string bindingString, int elementId)
        {
            return BindingStringHelper.GetCommandBindingData(elementId, bindingString);
        }

        public static PropertyBindingData ToPropertyBindingData(this string bindingString)
        {
            return BindingStringHelper.GetPropertyBindingData(bindingString);
        }
    }
}