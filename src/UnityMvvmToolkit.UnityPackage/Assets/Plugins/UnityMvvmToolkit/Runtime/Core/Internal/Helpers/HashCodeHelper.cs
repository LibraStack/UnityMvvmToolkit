using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Internal.Helpers
{
    internal static class HashCodeHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCode(int hash1, int hash2)
        {
            var hash = 17;
            hash = hash * 31 + hash1;
            hash = hash * 31 + hash2;

            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCode(int hash1, int hash2, int hash3)
        {
            return CombineHashCode(CombineHashCode(hash1, hash2), hash3);
        }
    }
}