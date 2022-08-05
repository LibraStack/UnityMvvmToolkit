using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Extensions;

namespace UnityMvvmToolkit.Common.Converters.ParameterConverters
{
    public class ParameterToFloatConverter : ParameterConverter<float>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryConvert(ReadOnlyMemory<char> parameter, out float result)
        {
            return parameter.Span.TryParse(out result);
        }
    }
}