using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Common.Converters.ParameterConverters
{
    public class ParameterToIntConverter : ParameterConverter<int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryConvert(ReadOnlyMemory<char> parameter, out int result)
        {
            return int.TryParse(parameter.Span, out result);
        }
    }
}