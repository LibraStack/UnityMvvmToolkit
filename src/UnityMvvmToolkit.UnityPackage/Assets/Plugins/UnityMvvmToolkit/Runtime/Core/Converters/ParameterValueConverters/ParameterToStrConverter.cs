using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Converters.ParameterValueConverters
{
    public class ParameterToStrConverter : ParameterValueConverter<string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string Convert(string parameter)
        {
            return parameter;
        }
    }
}