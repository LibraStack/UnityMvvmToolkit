using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Extensions;

namespace UnityMvvmToolkit.Common.Converters.ValueConverters
{
    public class FloatToStrConverter : ValueConverter<float, string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string Convert(float value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override float ConvertBack(string value)
        {
            value.AsSpan().TryParse(out var result);
            return result;
        }
    }
}