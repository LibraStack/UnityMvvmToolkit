using System.Globalization;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Extensions;

namespace UnityMvvmToolkit.Common.ValueConverters
{
    public class FloatToStrConverter : ValueConverter<float, string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryConvert(float value, out string result)
        {
            result = value.ToString(CultureInfo.CurrentCulture);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryConvertBack(string value, out float result)
        {
            return value.TryParse(out result);
        }
    }
}