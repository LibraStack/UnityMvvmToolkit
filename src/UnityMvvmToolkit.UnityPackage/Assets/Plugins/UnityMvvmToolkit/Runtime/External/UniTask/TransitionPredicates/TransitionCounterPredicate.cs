#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.TransitionPredicates
{
    using Interfaces;
    using UnityEngine.UIElements;

    public struct TransitionCounterPredicate : ITransitionPredicate
    {
        private int _transitionsCount;

        public TransitionCounterPredicate(int transitionsCount)
        {
            _transitionsCount = transitionsCount;
        }

        public bool TransitionEnd(TransitionEndEvent e) => --_transitionsCount == 0;
    }
}

#endif