using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Converters.PropertyValueConverters
{
    public sealed class IntToStrConverter : PropertyValueConverter<int, string>
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