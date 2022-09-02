#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.TransitionPredicates
{
    using Interfaces;
    using UnityEngine.UIElements;

    public readonly struct TransitionAnyPredicate : ITransitionPredicate
    {
        public bool TransitionEnd(TransitionEndEvent e) => true;
    }
}

#endif