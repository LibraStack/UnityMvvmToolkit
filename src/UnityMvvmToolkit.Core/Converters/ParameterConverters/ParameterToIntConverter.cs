using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Converters.ParameterConverters
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