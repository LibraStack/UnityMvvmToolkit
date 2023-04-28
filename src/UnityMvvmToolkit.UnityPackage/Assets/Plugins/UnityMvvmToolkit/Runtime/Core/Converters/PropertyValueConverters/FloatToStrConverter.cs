using System.Globalization;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Extensions;

namespace UnityMvvmToolkit.Core.Converters.PropertyValueConverters
{
    public sealed class FloatToStrConverter : PropertyValueConverter<float, string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string Convert(float value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override float ConvertBack(string value)
        {
            _ = value.TryParse(out var result);
            return result;
        }
    }
}