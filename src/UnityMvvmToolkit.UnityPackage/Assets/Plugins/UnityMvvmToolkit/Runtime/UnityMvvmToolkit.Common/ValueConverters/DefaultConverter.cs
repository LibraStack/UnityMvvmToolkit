using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.ValueConverters
{
    public class DefaultConverter : IValueConverter<string, string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryConvert(string value, out string result)
        {
            result = value;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryConvertBack(string value, out string result)
        {
            return TryConvert(value, out result);
        }
    }
}