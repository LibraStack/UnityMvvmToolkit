using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityMvvmToolkit.UITK.Extensions
{
    partial class VisualElementExtensions
    {
#if UNITY_2023_2_OR_NEWER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T As<T>(this object visualElement) where T : VisualElement
        {
            return (T) visualElement;
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T As<T>(this VisualElement visualElement) where T : VisualElement
        {
            return (T) visualElement;
        }
#endif
    }
}