using System.Runtime.CompilerServices;
using Enums;
using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;

namespace ValueConverters
{
    public class ThemeModeToBoolConverter : PropertyValueConverter<ThemeMode, bool>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Convert(ThemeMode value)
        {
            return (int) value == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ThemeMode ConvertBack(bool value)
        {
            return (ThemeMode) (value ? 1 : 0);
        }
    }
}