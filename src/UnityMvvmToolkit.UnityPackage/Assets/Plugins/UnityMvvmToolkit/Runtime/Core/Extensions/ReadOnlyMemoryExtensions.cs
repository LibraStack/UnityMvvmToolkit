using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Internal.Structs;

namespace UnityMvvmToolkit.Core.Extensions
{
    public static class ReadOnlyMemoryExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static LineSplitEnumerator Split(this ReadOnlyMemory<char> memory, char separator, bool trim = false)
        {
            return new LineSplitEnumerator(memory.Span, separator, trim);
        }
    }
}