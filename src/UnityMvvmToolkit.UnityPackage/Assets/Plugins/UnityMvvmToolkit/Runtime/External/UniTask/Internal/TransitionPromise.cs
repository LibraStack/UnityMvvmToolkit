#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.Internal
{
    using System;
    using Enums;
    using Interfaces;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UIElements;

    internal sealed class TransitionPromise<T> : IUniTaskSource<TransitionResult>, IPlayerLoopItem,
        ITaskPoolNode<TransitionPromise<T>> where T : ITransitionPredicate
    {
        private static TaskPool<TransitionPromise<T>> _pool;

        private TransitionPromise<T> _nextNode;
        private UniTaskCompletionSourceCore<TransitionResult> _core;

        private int _initFrame;
        private float _elapsed;
        private float _delayTime;

        private T _predicate;
        private bool _completed;
        private VisualElement _visualElement;
        private CancellationToken _cancellationToken;

        private readonly EventCallback<TransitionEndEvent> _endCallback;
        private readonly EventCallback<TransitionCancelEvent> _cancelCallback;

        private TransitionPromise()
        {
            _endCallback = OnTransitionEnd;
            _cancelCallback = OnTransitionCancel;
        }

        public ref TransitionPromise<T> NextNode => ref _nextNode;

        static TransitionPromise()
        {
            TaskPool.RegisterSizeGetter(typeof(TransitionPromise<T>), () => _pool.Size);
        }

        public static IUniTaskSource<TransitionResult> Create(VisualElement visualElement, T predicate, int timeoutMs,
            PlayerLoopTiming timing, CancellationToken cancellationToken, out short token)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return AutoResetUniTaskCompletionSource<TransitionResult>.CreateFromCanceled(cancellationToken, out token);
            }

            if (_pool.TryPop(out var transitionPromise) == false)
            {
                transitionPromise = new TransitionPromise<T>();
            }

            transitionPromise._elapsed = 0.0f;
            transitionPromise._delayTime = (float) TimeSpan.FromMilliseconds(timeoutMs).TotalSeconds;
            transitionPromise._initFrame = PlayerLoopHelper.IsMainThread ? Time.frameCount : -1;

            transitionPromise._completed = false;
            transitionPromise._predicate = predicate;
            transitionPromise._visualElement = visualElement;
            transitionPromise._cancellationToken = cancellationToken;

            TaskTracker.TrackActiveTask(transitionPromise, 3);
            PlayerLoopHelper.AddAction(timing, transitionPromise);

            visualElement.RegisterCallback(transitionPromise._cancelCallback);
            visualElement.RegisterCallback(transitionPromise._endCallback);

            token = transitionPromise._core.Version;

            return transitionPromise;
        }

        private void OnTransitionEnd(TransitionEndEvent e)
        {
            UnregisterCallbacks();

            if (_completed)
            {
                Reset();
                return;
            }

            _completed = true;

            if (_cancellationToken.IsCancellationRequested)
            {
                _core.TrySetCanceled(_cancellationToken);
            }
            else if (_predicate.TransitionEnd(e))
            {
                _core.TrySetResult(TransitionResult.Succeeded);
            }
        }

        private void OnTransitionCancel(TransitionCancelEvent e)
        {
            UnregisterCallbacks();

            if (_completed)
            {
                Reset();
                return;
            }

            _completed = true;

            if (_cancellationToken.IsCancellationRequested)
            {
                _core.TrySetCanceled(_cancellationToken);
            }
            else
            {
                _core.TrySetResult(TransitionResult.Canceled);
            }
        }

        private void UnregisterCallbacks()
        {
            _visualElement.UnregisterCallback(_cancelCallback);
            _visualElement.UnregisterCallback(_endCallback);
        }

        public TransitionResult GetResult(short token)
        {
            return _core.GetResult(token);
        }

        void IUniTaskSource.GetResult(short token)
        {
            GetResult(token);
        }

        public UniTaskStatus GetStatus(short token)
        {
            return _core.GetStatus(token);
        }

        public UniTaskStatus UnsafeGetStatus()
        {
            return _core.UnsafeGetStatus();
        }

        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            _core.OnCompleted(continuation, state, token);
        }

        public bool MoveNext()
        {
            if (_completed)
            {
                Reset();
                return false;
            }

            if (_cancellationToken.IsCancellationRequested)
            {
                _completed = true;
                _core.TrySetCanceled(_cancellationToken);

                return false;
            }

            if (_elapsed == 0.0f && _initFrame == Time.frameCount)
            {
                return true;
            }

            _elapsed += Time.deltaTime;

            if (_elapsed < _delayTime)
            {
                return true;
            }

            _completed = true;
            _core.TrySetResult(TransitionResult.Timeout);

            return false;
        }

        private void Reset()
        {
            TaskTracker.RemoveTracking(this);

            _core.Reset();

            _elapsed = default;
            _delayTime = default;

            _predicate = default;
            _visualElement = default;
            _cancellationToken = default;

            _pool.TryPush(this);
        }
    }
}

#endif