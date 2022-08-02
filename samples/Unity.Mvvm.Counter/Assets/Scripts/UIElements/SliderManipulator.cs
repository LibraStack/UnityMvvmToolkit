using UnityEngine;
using UnityEngine.UIElements;

namespace UIElements
{
    public delegate void PointerEventHandler(EventBase eventBase, Vector2 localPosition);

    public class SliderManipulator : InputManipulator
    {
        public event PointerEventHandler PointerDown;
        public event PointerEventHandler PointerMove;
        public event PointerEventHandler PointerUp;

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