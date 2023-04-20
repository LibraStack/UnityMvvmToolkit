using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Converters.ParameterValueConverters
{
    public sealed class ParameterToIntConverter : ParameterValueConverter<int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Convert(string parameter)
        {
            return int.Parse(parameter);
        }
    }
}