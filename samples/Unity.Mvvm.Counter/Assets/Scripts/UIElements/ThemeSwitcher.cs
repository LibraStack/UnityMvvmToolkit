using System;
using UnityEngine.UIElements;

namespace UIElements
{
    public class ThemeSwitcher : VisualElement
    {
        private const string ToggleSwitchClassName = "toggle-switch";
        private const string ToggleSwitchTrackClassName = "toggle-switch__track";
        private const string ToggleSwitchTrackThumbClassName = "toggle-switch__track__thumb";
        private const string ToggleSwitchLabelClassName = "toggle-switch__label";
        private const string ToggleSwitchLabelContainerClassName = "toggle-switch__label-container";

        private VisualElement _track;
        private VisualElement _thumb;

        private bool _isDarkMode;
        private bool _isLayoutCalculated;

        private float _thumbLeftPosition;
        private float _thumbRightPosition;

        public ThemeSwitcher()
        {
            AddToClassList(ToggleSwitchClassName);

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
            labelContainer.AddToClassList(ToggleSwitchLabelContainerClassName);

            var label = new Label();
            label.text = labelText;
            label.pickingMode = PickingMode.Ignore;
            label.AddToClassList(ToggleSwitchLabelClassName);
            label.AddToClassList($"{ToggleSwitchLabelClassName}{labelClassNameModifier}");

            labelContainer.Add(label);
            Add(labelContainer);
        }

        private void CreateTrack()
        {
            _track = new VisualElement();
            _track.name = "Track";
            _track.AddToClassList(ToggleSwitchTrackClassName);

            _thumb = new VisualElement();
            _thumb.name = "Thumb";
            _thumb.pickingMode = PickingMode.Ignore;
            _thumb.AddToClassList(ToggleSwitchTrackThumbClassName);

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