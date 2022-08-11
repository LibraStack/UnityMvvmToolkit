using System;
using UnityEngine.UIElements;

namespace UIElements
{
    public class CounterSlider : VisualElement
    {
        private const string SliderClassName = "slider";
        private const string SliderLabelClassName = "slider__label";
        private const string SliderThumbClassName = "slider__thumb";
        private const string SliderThumbIconClassName = "slider__thumb__icon";
        private const string SliderThumbAnimationClassName = "slider__thumb--animation";

        private VisualElement _thumb;
        private SliderManipulator _manipulator;

        public CounterSlider()
        {
            AddToClassList(SliderClassName);

            CreateLabel("LabelLeft", "−");
            CreateLabel("LabelRight", "+");
            CreateThumb();

            RegisterManipulator();
            RegisterCallback<GeometryChangedEvent>(OnLayoutCalculated);
        }

        public event EventHandler Increase
        {
            add => _manipulator.Increase += value;
            remove => _manipulator.Increase -= value;
        }

        public event EventHandler Decrease
        {
            add => _manipulator.Decrease += value;
            remove => _manipulator.Decrease -= value;
        }

        private void CreateLabel(string labelName, string labelText)
        {
            var label = new Label();
            label.text = labelText;
            label.name = labelName;
            label.pickingMode = PickingMode.Ignore;
            label.AddToClassList(SliderLabelClassName);

            Add(label);
        }

        private void CreateThumb()
        {
            _thumb = new VisualElement();
            _thumb.name = "Thumb";
            _thumb.AddToClassList(SliderThumbClassName);
            _thumb.AddToClassList(SliderThumbAnimationClassName);

            var thumbIcon = new VisualElement();
            thumbIcon.name = "Icon";
            thumbIcon.pickingMode = PickingMode.Ignore;
            thumbIcon.AddToClassList(SliderThumbIconClassName);

            _thumb.Add(thumbIcon);

            Add(_thumb);
        }

        private void RegisterManipulator()
        {
            _manipulator = new SliderManipulator(this, _thumb);
            this.AddManipulator(_manipulator);
        }

        private void OnLayoutCalculated(GeometryChangedEvent e)
        {
            _manipulator.Initialize();
        }

        public new class UxmlFactory : UxmlFactory<CounterSlider, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }
    }
}