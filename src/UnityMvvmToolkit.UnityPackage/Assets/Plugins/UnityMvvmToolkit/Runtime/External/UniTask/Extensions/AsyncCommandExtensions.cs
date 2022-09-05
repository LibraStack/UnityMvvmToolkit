#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.Extensions
{
    using System;
    using Internal;
    using Interfaces;

    public static class AsyncCommandExtensions
    {
        public static IAsyncCommand CreateCancelCommand(this IAsyncCommand asyncCommand)
        {
            if (asyncCommand == null)
            {
                throw new NullReferenceException(nameof(asyncCommand));
            }

            return new AsyncCommandWithCancellation(asyncCommand);
        }

        public static IAsyncCommand<T> CreateCancelCommand<T>(this IAsyncCommand<T> asyncCommand)
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