using UnityEngine;
using UnityEngine.UIElements;

namespace UIElements
{
    public delegate void MovableEventHandler(EventBase eventBase, Vector2 localPosition);
    
    public class MovableElement : Clickable
    {
        public event MovableEventHandler PointerDown;
        public event MovableEventHandler PointerMove;
        public event MovableEventHandler PointerUp;

        public MovableElement() : base(null, 250L, 30L)
        {
        }

        protected override void ProcessDownEvent(EventBase eventBase, Vector2 localPosition, int pointerId)
        {
            base.ProcessDownEvent(eventBase, localPosition, pointerId);
            PointerDown?.Invoke(eventBase, localPosition);

        }

        protected override void ProcessMoveEvent(EventBase eventBase, Vector2 localPosition)
        {
            base.ProcessMoveEvent(eventBase, localPosition);
            PointerMove?.Invoke(eventBase, localPosition);
        }

        protected override void ProcessUpEvent(EventBase eventBase, Vector2 localPosition, int pointerId)
        {
            base.ProcessUpEvent(eventBase, localPosition, pointerId);
            PointerUp?.Invoke(eventBase, localPosition);
        }
    }
}