using System;
using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Common.Converters.ParameterConverters
{
    public class ParameterToStrConverter : ParameterConverter<string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool TryConvert(ReadOnlyMemory<char> parameter, out string result)
        {
            result = parameter.ToString();
            return true;
        }
    }
}