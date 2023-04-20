using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.Extensions
{
    internal static class ObjectWrapperExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICommandWrapper AsCommandWrapper(this IObjectWrapper objectWrapper)
        {
            return (ICommandWrapper) objectWrapper;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IPropertyWrapper AsPropertyWrapper(this IObjectWrapper objectWrapper)
        {
            return (IPropertyWrapper) objectWrapper;
        }
    }
}