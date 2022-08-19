using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace Extensions
{
    public static class VisualElementExtensions
    {
        public static async UniTask WaitForTransitionFinish(this VisualElement element, int timeoutMs = 1000,
            CancellationToken cancellationToken = default)
        {
            if (AnyTransitionHasDuration(element) == false)
            {
                return;
            }

            var taskCompletionSource = new UniTaskCompletionSource();
            var cancellationTokenRegistration = cancellationToken.Register(() => taskCompletionSource.TrySetCanceled());

            void OnTransitionEnd(TransitionEndEvent e)
            {
                taskCompletionSource.TrySetResult();
            }

            void OnTransitionCancel(TransitionCancelEvent e)
            {
                taskCompletionSource.TrySetResult();
            }

            try
            {
                element.RegisterCallback<TransitionCancelEvent>(OnTransitionCancel);
                element.RegisterCallback<TransitionEndEvent>(OnTransitionEnd);

                var transitionTask = taskCompletionSource.Task;
                var timeoutTask = UniTask.Delay(timeoutMs, cancellationToken: cancellationToken);

                var completedTaskIndex = await UniTask.WhenAny(transitionTask, timeoutTask);

                if (completedTaskIndex != 0)
                {
                    taskCompletionSource.TrySetCanceled();
                }
            }
            finally
            {
                await cancellationTokenRegistration.DisposeAsync();

                element.UnregisterCallback<TransitionCancelEvent>(OnTransitionCancel);
                element.UnregisterCallback<TransitionEndEvent>(OnTransitionEnd);
            }
        }

        private static bool AnyTransitionHasDuration(VisualElement element)
        {
            return element.resolvedStyle.transitionDuration.Any(duration => duration.value > float.Epsilon);
        }
    }
}