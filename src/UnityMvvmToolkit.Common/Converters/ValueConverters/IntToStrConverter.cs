using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Common.Converters.ValueConverters
{
    public class IntToStrConverter : ValueConverter<int, string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryConvert(int value, out string result)
        {
            result = value.ToString();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryConvertBack(string value, out int result)
        {
            return int.TryParse(value, out result);
        }
    }
}