using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Common.Converters.ParameterConverters
{
    public class ParameterToStrConverter : ParameterConverter<string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string Convert(ReadOnlyMemory<char> parameter)
        {
            return parameter.ToString();
        }
    }
}