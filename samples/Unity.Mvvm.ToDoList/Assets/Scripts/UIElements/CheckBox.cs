using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UIElements
{
    public class CheckBox : VisualElement
    {
        private const string CheckBoxClassName = "check-box";

        private const string LabelDoneClassName = "check-box__label--done";
        private const string StateCheckDoneClassName = "check-box__tick--done";
        private const string StateCircleDoneClassName = "check-box__circle--done";

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetIsChecked(bool value, bool notify)
        {
            _isChecked = value;
            UpdateVisuals(value);

            if (notify)
            {
                IsCheckedChanged?.Invoke(this, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateVisuals(bool value)
        {
            if (value)
            {
                _label.AddToClassList(LabelDoneClassName);
                _tick.AddToClassList(StateCheckDoneClassName);
                _circle.AddToClassList(StateCircleDoneClassName);
            }
            else
            {
                _label.RemoveFromClassList(LabelDoneClassName);
                _tick.RemoveFromClassList(StateCheckDoneClassName);
                _circle.RemoveFromClassList(StateCircleDoneClassName);
            }
        }

        private void CreateToggle()
        {
            _clickableArea = new VisualElement();
            _clickableArea.name = "ClickableArea";
            _clickableArea.AddToClassList("check-box__clickable-area");
            _clickableArea.AddToClassList("check-box__clickable-area--animation");
            _clickableArea.RegisterCallback<ClickEvent>(OnToggleClick);

            _circle = new VisualElement();
            _circle.name = "Circle";
            _circle.AddToClassList("check-box__circle");

            _tick = new VisualElement();
            _tick.name = "Tick";
            _tick.AddToClassList("check-box__tick");
            _tick.AddToClassList("check-box__tick--animation");

            _circle.Add(_tick);
            _clickableArea.Add(_circle);

            Add(_clickableArea);
        }

        private void CreateLabel()
        {
            _label = new Label();
            _label.name = "Label";
            _label.text = "Select 10 Items";
            _label.AddToClassList("check-box__label");
            _label.AddToClassList("check-box__label--animation");

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