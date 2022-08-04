using System.Runtime.CompilerServices;
using Enums;
using UnityMvvmToolkit.Common.ValueConverters;

namespace ValueConverters
{
    public class ThemeModeToBoolConverter : ValueConverter<ThemeMode, bool>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryConvert(ThemeMode value, out bool result)
        {
            result = (int) value == 1;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryConvertBack(bool value, out ThemeMode result)
        {
            result = (ThemeMode) (value ? 1 : 0);
            return true;
        }
    }
}