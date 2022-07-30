using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIElements
{
    public class CounterSlider : VisualElement
    {
        private const string ThumbAnimationClassName = "slider__thumb--animation";

        private bool _isDragMode;
        private float _decreasePositionX;
        private float _increasePositionX;

        private VisualElement _thumb;

        public CounterSlider()
        {
            AddToClassList("slider");

            CreateLabel("LabelLeft", "−");
            CreateLabel("LabelRight", "+");
            CreateThumb();

            RegisterManipulator();
            RegisterCallback<GeometryChangedEvent>(OnLayoutCalculated);
        }

        public event EventHandler Increase;
        public event EventHandler Decrease;

        private void CreateLabel(string labelName, string labelText)
        {
            var label = new Label();
            label.text = labelText;
            label.name = labelName;
            label.AddToClassList("slider__label");

            Add(label);
        }

        private void CreateThumb()
        {
            _thumb = new VisualElement();
            _thumb.name = "Thumb";
            _thumb.AddToClassList("slider__thumb");
            _thumb.AddToClassList("slider__thumb--animation");

            var thumbIcon = new VisualElement();
            thumbIcon.name = "Icon";
            thumbIcon.pickingMode = PickingMode.Ignore;
            thumbIcon.AddToClassList("slider__thumb__icon");

            _thumb.Add(thumbIcon);

            Add(_thumb);
        }

        private void RegisterManipulator()
        {
            var clickableElement = new MovableElement();
            clickableElement.PointerDown += OnPointerDown;
            clickableElement.PointerMove += OnPointerMove;
            clickableElement.PointerUp += OnPointerUp;

            this.AddManipulator(clickableElement);
        }

        private void OnLayoutCalculated(GeometryChangedEvent e)
        {
            _decreasePositionX = _thumb.resolvedStyle.left - _thumb.resolvedStyle.width / 2;
            _increasePositionX = _thumb.resolvedStyle.left + _thumb.resolvedStyle.width / 2;
        }

        private void OnPointerDown(EventBase eventBase, Vector2 localPosition)
        {
            if (eventBase.target == _thumb)
            {
                _isDragMode = true;
                BeginThumbMove(localPosition);
            }
        }

        private void OnPointerMove(EventBase eventBase, Vector2 localPosition)
        {
            if (_isDragMode)
            {
                SetThumbPosition(localPosition.x);
            }
        }

        private void OnPointerUp(EventBase eventBase, Vector2 localPosition)
        {
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
            SetThumbPosition(resolvedStyle.width / 2);

            if (localPosition.x >= _increasePositionX)
            {
                Increase?.Invoke(this, EventArgs.Empty);
            }
            else if (localPosition.x <= _decreasePositionX)
            {
                Decrease?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SetThumbPosition(float value)
        {
            _thumb.style.left = value;
        }

        public new class UxmlFactory : UxmlFactory<CounterSlider, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }
    }
}