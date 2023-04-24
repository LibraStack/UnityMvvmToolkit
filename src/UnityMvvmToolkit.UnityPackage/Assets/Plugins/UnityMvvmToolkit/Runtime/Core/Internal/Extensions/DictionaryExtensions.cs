using System.Collections.Generic;

namespace UnityMvvmToolkit.Core.Internal.Extensions
{
    internal static class ImmutableDictionary
    {
        internal static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>()
        {
            return EmptyDictionary<TKey, TValue>.Value;
        }

        private static class EmptyDictionary<TKey, TValue>
        {
            internal static readonly IReadOnlyDictionary<TKey, TValue> Value = new Dictionary<TKey, TValue>();
        }
    }
}