using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Common.Converters.ParameterConverters
{
    public class ParameterToIntConverter : ParameterConverter<int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Convert(ReadOnlyMemory<char> parameter)
        {
            return int.Parse(parameter.Span);
        }
    }
}