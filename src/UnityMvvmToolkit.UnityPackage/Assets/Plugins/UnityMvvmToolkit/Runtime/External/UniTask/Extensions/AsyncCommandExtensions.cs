#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.Extensions
{
    using System;
    using Internal;
    using Interfaces;

    public static class AsyncCommandExtensions
    {
        public static IAsyncCommand WithCancellation(this IAsyncCommand asyncCommand)
        {
            if (asyncCommand == null)
            {
                throw new NullReferenceException(nameof(asyncCommand));
            }

            return new AsyncCommandWithCancellation(asyncCommand);
        }

        public static IAsyncCommand<T> WithCancellation<T>(this IAsyncCommand<T> asyncCommand)
        {
            if (asyncCommand == null)
            {
                throw new NullReferenceException(nameof(asyncCommand));
            }

            return new AsyncCommandWithCancellation<T>(asyncCommand);
        }
    }
}

#endif