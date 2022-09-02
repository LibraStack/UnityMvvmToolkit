#if UNITYMVVMTOOLKIT_UNITASK_SUPPORT

namespace UnityMvvmToolkit.UniTask.Interfaces
{
    using UnityEngine.UIElements;
    
    public interface ITransitionPredicate
    {
        bool TransitionEnd(TransitionEndEvent e);
    }
}

#endif