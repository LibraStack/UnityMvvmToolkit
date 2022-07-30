using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Extensions;
using UnityMvvmToolkit.Common.Interfaces;

namespace UnityMvvmToolkit.Common.ValueConverters
{
    public class IntToStrConverter : IValueConverter<int, string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryConvert(int value, out string result)
        {
            result = value.ToString();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryConvertBack(string value, out int result)
        {
            return value.AsSpan().TryParse(out result); // TODO: Benchmark.
        }
    }
}