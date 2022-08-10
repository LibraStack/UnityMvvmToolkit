using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Converters.ParameterValueConverters
{
    public class ParameterToIntConverter : ParameterValueConverter<int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Convert(ReadOnlyMemory<char> parameter)
        {
            return int.Parse(parameter.Span);
        }
    }
}