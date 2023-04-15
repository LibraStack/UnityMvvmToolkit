using UnityEngine;
using UnityEngine.UIElements;

namespace Manipulators
{
    public abstract class InputManipulator : PointerManipulator
    {
        private bool _isActive;

        protected InputManipulator()
        {
            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOut);

            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerCancelEvent>(OnPointerCancel);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOut);

            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerCancelEvent>(OnPointerCancel);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            if (CanStartManipulation(e))
            {
                ProcessDownEvent(e, e.localMousePosition, PointerId.mousePointerId);
            }
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            if (_isActive)
            {
                ProcessMoveEvent(e, e.localMousePosition);
            }
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (_isActive && CanStopManipulation(e))
            {
                ProcessUpEvent(e, e.localMousePosition, PointerId.mousePointerId);
            }
        }

        private void OnMouseCaptureOut(MouseCaptureOutEvent e)
        {
            if (_isActive)
            {
                ProcessCancelEvent(e, PointerId.mousePointerId);
            }
        }

        private void OnPointerDown(PointerDownEvent e)
        {
            if (CanStartManipulation(e) == false)
            {
                return;
            }

            if (IsMouse(e.pointerId))
            {
                e.StopImmediatePropagation();
            }
            else
            {
                ProcessDownEvent(e, e.localPosition, e.pointerId);
            }
        }

        private void OnPointerMove(PointerMoveEvent e)
        {
            if (_isActive == false)
            {
                return;
            }

            if (IsMouse(e.pointerId))
            {
                e.StopPropagation();
            }
            else
            {
                ProcessMoveEvent(e, e.localPosition);
            }
        }

        private void OnPointerUp(PointerUpEvent e)
        {
            if (_isActive == false || CanStopManipulation(e) == false)
            {
                return;
            }

            if (IsMouse(e.pointerId))
            {
                e.StopPropagation();
            }
            else
            {
                ProcessUpEvent(e, e.localPosition, e.pointerId);
            }
        }

        private void OnPointerCancel(PointerCancelEvent e)
        {
            if (_isActive && CanStopManipulation(e) && IsMouse(e.pointerId) == false)
            {
                ProcessCancelEvent(e, e.pointerId);
            }
        }

        private void OnPointerCaptureOut(PointerCaptureOutEvent e)
        {
            if (_isActive && IsMouse(e.pointerId) == false)
            {
                ProcessCancelEvent(e, e.pointerId);
            }
        }

        private bool IsMouse(int pointerId)
        {
            return pointerId == PointerId.mousePointerId;
        }

        protected virtual void ProcessDownEvent(EventBase e, Vector2 localPosition, int pointerId)
        {
            _isActive = true;
            target.CapturePointer(pointerId);

            e.StopImmediatePropagation();
        }

        protected virtual void ProcessMoveEvent(EventBase e, Vector2 localPosition)
        {
            e.StopPropagation();
        }

        protected virtual void ProcessUpEvent(EventBase e, Vector2 localPosition, int pointerId)
        {
            _isActive = false;
            target.ReleasePointer(pointerId);

            e.StopPropagation();
        }

        protected virtual void ProcessCancelEvent(EventBase e, int pointerId)
        {
            _isActive = false;
            target.ReleasePointer(pointerId);

            e.StopPropagation();
        }
    }
}