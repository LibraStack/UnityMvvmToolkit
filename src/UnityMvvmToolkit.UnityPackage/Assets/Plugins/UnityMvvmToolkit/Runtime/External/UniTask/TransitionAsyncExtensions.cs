#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using System;
    using System.Linq;
    using System.Threading;
    using Interfaces;
    using TransitionPredicates;
    using UnityEngine.UIElements;
    using Cysharp.Threading.Tasks;
    using System.Runtime.CompilerServices;

    public static class TransitionAsyncExtensions
    {
        private const char Dash = '-';
        private const int DefaultTimeoutMs = 2500;

        public static async UniTask WaitForLongestTransitionEnd(this VisualElement element,
            CancellationToken cancellationToken = default)
        {
            var maxTransitionDelay = element.resolvedStyle.transitionDuration.Max(duration =>
                duration.unit == TimeUnit.Millisecond ? duration.value : duration.value * 1000);

            if (maxTransitionDelay > float.Epsilon)
            {
                await UniTask.Delay((int) maxTransitionDelay, cancellationToken: cancellationToken);
            }
        }

        public static async UniTask WaitForTransitionEnd(this VisualElement element, int transitionIndex,
            int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
        {
            if (GetTransitionDuration(element, transitionIndex, out var stylePropertyName) > float.Epsilon)
            {
                await WaitForTransition(element, stylePropertyName, timeoutMs, cancellationToken);
            }
        }

        public static async UniTask WaitForTransitionEnd(this VisualElement element, StylePropertyName stylePropertyName,
            int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
        {
            if (GetTransitionDuration(element, stylePropertyName) > float.Epsilon)
            {
                await WaitForTransition(element, stylePropertyName, timeoutMs, cancellationToken);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="propertyName">CamelCase propertyName: nameof(propertyName).</param>
        /// <param name="timeoutMs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async UniTask WaitForTransitionEnd(this VisualElement element, string propertyName,
            int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
        {
            var transitionData = GetTransitionData(element, propertyName);
            if (transitionData is { Duration: { value: > float.Epsilon } })
            {
                await WaitForTransition(element, transitionData.Value.StylePropertyName, timeoutMs, cancellationToken);
            }
        }

        public static async UniTask WaitForAnyTransitionEnd(this VisualElement element, int timeoutMs = DefaultTimeoutMs,
            CancellationToken cancellationToken = default)
        {
            if (AnyTransitionHasDuration(element))
            {
                await WaitForTransitionEnd(element, new TransitionAnyPredicate(), timeoutMs, cancellationToken);
            }
        }

        public static async UniTask WaitForAllTransitionsEnd(this VisualElement element, int timeoutMs = DefaultTimeoutMs,
            CancellationToken cancellationToken = default)
        {
            var transitionsCount = GetTransitionsWithDurationCount(element);
            if (transitionsCount == 0)
            {
                return;
            }

            await WaitForTransitionEnd(element, new TransitionCounterPredicate(transitionsCount), timeoutMs,
                cancellationToken);
        }

        public static async UniTask WaitForTransitionEnd<T>(this VisualElement element, T transitionPredicate,
            int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
            where T : ITransitionPredicate
        {
            var taskCompletionSource = new UniTaskCompletionSource();

            void OnTransitionEnd(TransitionEndEvent e)
            {
                if (transitionPredicate.TransitionEnd(e))
                {
                    taskCompletionSource.TrySetResult();
                }
            }

            void OnTransitionCancel(TransitionCancelEvent e)
            {
                if (transitionPredicate.TransitionCancel(e))
                {
                    taskCompletionSource.TrySetCanceled();
                }
            }

            try
            {
                element.RegisterCallback<TransitionCancelEvent>(OnTransitionCancel);
                element.RegisterCallback<TransitionEndEvent>(OnTransitionEnd);

                var transitionTask = taskCompletionSource.Task;
                var timeoutTask = UniTask.Delay(timeoutMs, cancellationToken: cancellationToken);

                var task = await UniTask.WhenAny(transitionTask, timeoutTask).SuppressCancellationThrow();
                if (task.IsCanceled || task.Result == 1)
                {
                    taskCompletionSource.TrySetCanceled();
                }
            }
            finally
            {
                element.UnregisterCallback<TransitionCancelEvent>(OnTransitionCancel);
                element.UnregisterCallback<TransitionEndEvent>(OnTransitionEnd);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UniTask WaitForTransition(this VisualElement element, StylePropertyName stylePropertyName,
            int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
        {
            return WaitForTransitionEnd(element, new TransitionNamePredicate(stylePropertyName), timeoutMs,
                cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AnyTransitionHasDuration(VisualElement element)
        {
            return element.resolvedStyle.transitionDuration.Any(duration => duration.value > float.Epsilon);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetTransitionsWithDurationCount(VisualElement element)
        {
            return element.resolvedStyle.transitionDuration.Count(duration => duration.value > float.Epsilon);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetTransitionDuration(VisualElement element, int transitionIndex,
            out StylePropertyName stylePropertyName)
        {
            using var properties = element.resolvedStyle.transitionProperty.GetEnumerator();
            using var durations = element.resolvedStyle.transitionDuration.GetEnumerator();

            var index = 0;

            while (properties.MoveNext() && durations.MoveNext())
            {
                if (index == transitionIndex)
                {
                    stylePropertyName = properties.Current;
                    return durations.Current.value;
                }

                index++;
            }

            stylePropertyName = default;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetTransitionDuration(VisualElement element, StylePropertyName stylePropertyName)
        {
            using var properties = element.resolvedStyle.transitionProperty.GetEnumerator();
            using var durations = element.resolvedStyle.transitionDuration.GetEnumerator();

            while (properties.MoveNext() && durations.MoveNext())
            {
                if (properties.Current == stylePropertyName)
                {
                    return durations.Current.value;
                }
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (StylePropertyName StylePropertyName, TimeValue Duration)? GetTransitionData(VisualElement element,
            ReadOnlySpan<char> propertyName)
        {
            using var properties = element.resolvedStyle.transitionProperty.GetEnumerator();
            using var durations = element.resolvedStyle.transitionDuration.GetEnumerator();

            while (properties.MoveNext() && durations.MoveNext())
            {
                if (LettersEqual(properties.Current.ToString(), propertyName))
                {
                    return (properties.Current, durations.Current);
                }
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool LettersEqual(ReadOnlySpan<char> dashCase, ReadOnlySpan<char> camelCase)
        {
            if (camelCase.Length > dashCase.Length)
            {
                return false;
            }

            var camelIndex = 0;

            for (var i = 0; i < dashCase.Length; i++)
            {
                var dashChar = dashCase[i];
                if (dashChar == Dash)
                {
                    continue;
                }

                if (dashChar != char.ToLowerInvariant(camelCase[camelIndex]))
                {
                    return false;
                }

                camelIndex++;
            }

            return true;
        }
    }
}

#endif