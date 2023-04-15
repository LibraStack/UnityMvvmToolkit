using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Manipulators
{
    public class SliderManipulator : InputManipulator
    {
        private const string ThumbAnimationClassName = "slider__thumb--animation";

        private bool _isDragMode;
        private float _decreasePositionX;
        private float _increasePositionX;

        private readonly VisualElement _slider;
        private readonly VisualElement _thumb;

        public SliderManipulator(VisualElement slider, VisualElement thumb)
        {
            _slider = slider;
            _thumb = thumb;
        }

        public event EventHandler Increment;
        public event EventHandler Decrement;

        public void Initialize()
        {
            _decreasePositionX = _thumb.resolvedStyle.left - _thumb.resolvedStyle.width / 2;
            _increasePositionX = _thumb.resolvedStyle.left + _thumb.resolvedStyle.width / 2;
        }

        protected override void ProcessDownEvent(EventBase eventBase, Vector2 localPosition, int pointerId)
        {
            base.ProcessDownEvent(eventBase, localPosition, pointerId);

            if (eventBase.target == _thumb)
            {
                _isDragMode = true;
                BeginThumbMove(localPosition);
            }
        }

        protected override void ProcessMoveEvent(EventBase eventBase, Vector2 localPosition)
        {
            base.ProcessMoveEvent(eventBase, localPosition);

            if (_isDragMode)
            {
                SetThumbPosition(localPosition.x);
            }
        }

        protected override void ProcessUpEvent(EventBase eventBase, Vector2 localPosition, int pointerId)
        {
            base.ProcessUpEvent(eventBase, localPosition, pointerId);

            if (_isDragMode)
            {
                _isDragMode = false;
                EndThumbMove(localPosition).Forget();
            }
        }

        private void BeginThumbMove(Vector2 localPosition)
        {
            _thumb.RemoveFromClassList(ThumbAnimationClassName);
            SetThumbPosition(localPosition.x);
        }

        private async UniTaskVoid EndThumbMove(Vector2 localPosition)
        {
            _thumb.AddToClassList(ThumbAnimationClassName);
            await UniTask.Yield();
            SetThumbPosition(_slider.resolvedStyle.width / 2);

            if (localPosition.x >= _increasePositionX)
            {
                Increment?.Invoke(this, EventArgs.Empty);
            }
            else if (localPosition.x <= _decreasePositionX)
            {
                Decrement?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SetThumbPosition(float value)
        {
            _thumb.style.left = value;
        }
    }
}