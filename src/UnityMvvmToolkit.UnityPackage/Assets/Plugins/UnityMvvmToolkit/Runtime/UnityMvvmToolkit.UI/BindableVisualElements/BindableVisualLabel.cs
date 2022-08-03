using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI.BindableUIElements;

namespace UnityMvvmToolkit.UI.BindableVisualElements
{
    public class BindableVisualLabel : BindableVisualElement
    {
        private readonly BindableLabel _label;
        private readonly IReadOnlyProperty<string> _textProperty;

        public BindableVisualLabel(BindableLabel label, IPropertyProvider propertyProvider) : base(propertyProvider)
        {
            _label = label;
            _textProperty = GetReadOnlyProperty<string>(label.BindingTextPath);
        }

        public override void UpdateValues()
        {
            if (_textProperty != null)
            {
                _label.text = _textProperty.Value;
            }
        }
    }
}