#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask
{
    using System;
    using System.Linq;
    using System.Threading;
    using Enums;
    using Internal;
    using Interfaces;
    using TransitionPredicates;
    using UnityEngine.UIElements;
    using Cysharp.Threading.Tasks;
    using System.Runtime.CompilerServices;

    public static class TransitionAsyncExtensions
    {
        private const char Dash = '-';
        private const int DefaultTimeoutMs = 2500;

        public static UniTask WaitForLongestTransitionEnd(this VisualElement element,
            CancellationToken cancellationToken = default)
        {
            var maxTransitionDelay = element.resolvedStyle.transitionDuration.Max(duration =>
                duration.unit == TimeUnit.Millisecond ? duration.value : duration.value * 1000);

            return maxTransitionDelay > float.Epsilon
                ? UniTask.Delay((int) maxTransitionDelay, cancellationToken: cancellationToken)
                : UniTask.CompletedTask;
        }

        public static UniTask<TransitionResult> WaitForTransitionEnd(this VisualElement element, int transitionIndex,
            int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
        {
            return GetTransitionDuration(element, transitionIndex, out var stylePropertyName) > float.Epsilon
                ? WaitForTransition(element, stylePropertyName, timeoutMs, cancellationToken)
                : UniTask.FromResult(TransitionResult.Missed);
        }

        public static UniTask<TransitionResult> WaitForTransitionEnd(this VisualElement element,
            StylePropertyName stylePropertyName, int timeoutMs = DefaultTimeoutMs,
            CancellationToken cancellationToken = default)
        {
            return GetTransitionDuration(element, stylePropertyName) > float.Epsilon
                ? WaitForTransition(element, stylePropertyName, timeoutMs, cancellationToken)
                : UniTask.FromResult(TransitionResult.Missed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="propertyName">CamelCase propertyName: nameof(propertyName).</param>
        /// <param name="timeoutMs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static UniTask<TransitionResult> WaitForTransitionEnd(this VisualElement element, string propertyName,
            int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
        {
            var transitionData = GetTransitionData(element, propertyName);

            return transitionData is { Duration: { value: > float.Epsilon } }
                ? WaitForTransition(element, transitionData.Value.StylePropertyName, timeoutMs, cancellationToken)
                : UniTask.FromResult(TransitionResult.Missed);
        }

        public static UniTask<TransitionResult> WaitForAnyTransitionEnd(this VisualElement element,
            int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
        {
            return AnyTransitionHasDuration(element)
                ? WaitForTransitionEnd(element, new TransitionAnyPredicate(), timeoutMs, cancellationToken)
                : UniTask.FromResult(TransitionResult.Missed);
        }

        public static UniTask<TransitionResult> WaitForAllTransitionsEnd(this VisualElement element,
            int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
        {
            var transitionsCount = GetTransitionsWithDurationCount(element);

            return transitionsCount == 0
                ? UniTask.FromResult(TransitionResult.Missed)
                : WaitForTransitionEnd(element, new TransitionCounterPredicate(transitionsCount), timeoutMs,
                    cancellationToken);
        }

        public static UniTask<TransitionResult> WaitForTransitionEnd<T>(this VisualElement element,
            T transitionPredicate, int timeoutMs = DefaultTimeoutMs, CancellationToken cancellationToken = default)
            where T : ITransitionPredicate
        {
            return new UniTask<TransitionResult>(
                TransitionPromise<T>.Create(element, transitionPredicate, timeoutMs, PlayerLoopTiming.Update,
                    cancellationToken, out var token), token);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UniTask<TransitionResult> WaitForTransition(this VisualElement element,
            StylePropertyName stylePropertyName, int timeoutMs = DefaultTimeoutMs,
            CancellationToken cancellationToken = default)
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