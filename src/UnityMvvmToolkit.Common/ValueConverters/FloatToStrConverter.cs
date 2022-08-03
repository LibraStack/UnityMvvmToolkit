using System.Globalization;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.ValueConverters
{
    public class FloatToStrConverter : IValueConverter<float, string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryConvert(float value, out string result)
        {
            result = value.ToString(CultureInfo.CurrentCulture);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryConvertBack(string value, out float result)
        {
            return value.TryParse(out result);
        }
    }
}