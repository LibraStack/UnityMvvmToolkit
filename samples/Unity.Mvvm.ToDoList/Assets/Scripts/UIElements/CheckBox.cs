using System;
using UnityEngine.UIElements;

namespace UIElements
{
    public class CheckBox : VisualElement
    {
        private const string CheckBoxClassName = "check-box";

        private const string TickClassName = "check-box__tick";
        private const string TickDoneClassName = "check-box__tick--done";
        private const string TickAnimationClassName = "check-box__tick--animation";

        private const string CircleClassName = "check-box__circle";
        private const string CircleDoneClassName = "check-box__circle--done";

        private const string LabelClassName = "check-box__label";
        private const string LabelDoneClassName = "check-box__label--done";
        private const string LabelAnimationClassName = "check-box__label--animation";

        private const string ClickableAreaClassName = "check-box__clickable-area";
        private const string ClickableAreaAnimationClassName = "check-box__clickable-area--animation";

        private bool _isChecked;

        private Label _label;

        private VisualElement _tick;
        private VisualElement _circle;
        private VisualElement _clickableArea;

        public CheckBox()
        {
            AddToClassList(CheckBoxClassName);

            CreateToggle();
            CreateLabel();
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    SetIsChecked(value, true);
                }
            }
        }

        public string Text
        {
            get => _label.text;
            set => _label.text = value;
        }

        public event EventHandler<bool> IsCheckedChanged;

        protected void SetIsCheckedWithoutNotify(bool isChecked)
        {
            SetIsChecked(isChecked, false);
        }

        private void OnToggleClick(ClickEvent e)
        {
            e.StopImmediatePropagation();
            IsChecked = !IsChecked;
        }

        private void SetIsChecked(bool value, bool notify)
        {
            _isChecked = value;
            UpdateVisuals(value);

            if (notify)
            {
                IsCheckedChanged?.Invoke(this, value);
            }
        }

        private void UpdateVisuals(bool value)
        {
            if (value)
            {
                _label.AddToClassList(LabelDoneClassName);
                _tick.AddToClassList(TickDoneClassName);
                _circle.AddToClassList(CircleDoneClassName);
            }
            else
            {
                _label.RemoveFromClassList(LabelDoneClassName);
                _tick.RemoveFromClassList(TickDoneClassName);
                _circle.RemoveFromClassList(CircleDoneClassName);
            }
        }

        private void CreateToggle()
        {
            _clickableArea = new VisualElement();
            _clickableArea.name = "ClickableArea";
            _clickableArea.AddToClassList(ClickableAreaClassName);
            _clickableArea.AddToClassList(ClickableAreaAnimationClassName);
            _clickableArea.RegisterCallback<ClickEvent>(OnToggleClick);

            _circle = new VisualElement();
            _circle.name = "Circle";
            _circle.AddToClassList(CircleClassName);

            _tick = new VisualElement();
            _tick.name = "Tick";
            _tick.AddToClassList(TickClassName);
            _tick.AddToClassList(TickAnimationClassName);

            _circle.Add(_tick);
            _clickableArea.Add(_circle);

            Add(_clickableArea);
        }

        private void CreateLabel()
        {
            _label = new Label();
            _label.name = "Label";
            _label.text = GetType().Name;
            _label.AddToClassList(LabelClassName);
            _label.AddToClassList(LabelAnimationClassName);

            Add(_label);
        }

        public new class UxmlFactory : UxmlFactory<CheckBox, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _textAttribute = new()
                { name = "text", defaultValue = "Check Box" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((CheckBox) visualElement).Text = _textAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}