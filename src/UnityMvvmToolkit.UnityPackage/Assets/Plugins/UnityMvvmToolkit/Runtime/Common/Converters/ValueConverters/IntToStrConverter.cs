using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Common.Converters.ValueConverters
{
    public class IntToStrConverter : ValueConverter<int, string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string Convert(int value)
        {
            return value.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int ConvertBack(string value)
        {
            return int.Parse(value);
        }
    }
}