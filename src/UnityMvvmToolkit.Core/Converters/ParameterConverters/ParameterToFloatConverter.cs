using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Extensions;

namespace UnityMvvmToolkit.Core.Converters.ParameterConverters
{
    public class ParameterToFloatConverter : ParameterConverter<float>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override float Convert(ReadOnlyMemory<char> parameter)
        {
            parameter.Span.TryParse(out var result);
            return result;
        }
    }
}