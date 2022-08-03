using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.ValueConverters
{
    public class IntToStrConverter : IValueConverter<int, string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryConvert(int value, out string result)
        {
            result = value.ToString();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryConvertBack(string value, out int result)
        {
            return int.TryParse(value, out result);
        }
    }
}