using System;
using UnityEngine.UIElements;

namespace UIElements
{
    public class ThemeSwitcher : VisualElement
    {
        private VisualElement _track;
        private VisualElement _thumb;

        private bool _isDarkMode;
        private bool _isLayoutCalculated;

        private float _thumbLeftPosition;
        private float _thumbRightPosition;

        public ThemeSwitcher()
        {
            AddToClassList("toggle-switch");

            CreateLabelContainer("LeftContainer", "Light", "--left");
            CreateTrack();
            CreateLabelContainer("RightContainer", "Dark", "--right");

            RegisterCallback<GeometryChangedEvent>(OnLayoutCalculated);
            RegisterCallback<ClickEvent>(OnClick);
        }

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set => SetValue(value);
        }

        public event EventHandler<bool> Switch;

        public void SetValueWithoutNotify(bool value)
        {
            SetValue(value, false);
        }

        private void CreateLabelContainer(string containerName, string labelText, string labelClassNameModifier)
        {
            var labelContainer = new VisualElement();
            labelContainer.name = containerName;
            labelContainer.pickingMode = PickingMode.Ignore;
            labelContainer.AddToClassList("toggle-switch__label-container");

            var label = new Label();
            label.text = labelText;
            label.pickingMode = PickingMode.Ignore;
            label.AddToClassList("toggle-switch__label");
            label.AddToClassList($"toggle-switch__label{labelClassNameModifier}");

            labelContainer.Add(label);
            Add(labelContainer);
        }

        private void CreateTrack()
        {
            _track = new VisualElement();
            _track.name = "Track";
            _track.AddToClassList("toggle-switch__track");

            _thumb = new VisualElement();
            _thumb.name = "Thumb";
            _thumb.pickingMode = PickingMode.Ignore;
            _thumb.AddToClassList("toggle-switch__track__thumb");

            _track.Add(_thumb);
            Add(_track);
        }

        private void OnLayoutCalculated(GeometryChangedEvent e)
        {
            _thumbLeftPosition = _thumb.resolvedStyle.width / 2;
            _thumbRightPosition = _track.resolvedStyle.width - _thumb.resolvedStyle.width +
                                  _thumb.resolvedStyle.marginRight;
            _isLayoutCalculated = true;

            UpdateThumbPosition();
        }

        private void OnClick(ClickEvent e)
        {
            e.StopImmediatePropagation();
            IsDarkMode = !IsDarkMode;
        }

        private void SetValue(bool value, bool notify = true)
        {
            if (_isDarkMode == value)
            {
                return;
            }

            _isDarkMode = value;
            UpdateThumbPosition();

            if (notify)
            {
                Switch?.Invoke(this, value);
            }
        }

        private void UpdateThumbPosition()
        {
            if (_isLayoutCalculated)
            {
                _thumb.style.left = _isDarkMode ? _thumbRightPosition : _thumbLeftPosition;
            }
        }

        public new class UxmlFactory : UxmlFactory<ThemeSwitcher, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }
    }
}