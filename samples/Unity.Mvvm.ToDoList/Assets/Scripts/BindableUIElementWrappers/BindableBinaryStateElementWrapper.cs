using Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElementWrappers
{
    public class BindableBinaryStateElementWrapper : BindablePropertyElement
    {
        private readonly IBindableBinaryStateElement _binaryStateElement;
        private readonly IReadOnlyProperty<bool> _stateProperty;

        public BindableBinaryStateElementWrapper(IBindableBinaryStateElement binaryStateElement,
            IObjectProvider objectProvider) : base(objectProvider)
        {
            _binaryStateElement = binaryStateElement;
            _stateProperty = GetReadOnlyProperty<bool>(binaryStateElement.BindingStatePath);
        }

        public override void UpdateValues()
        {
            if (_stateProperty.Value)
            {
                _binaryStateElement.Activate();
            }
            else
            {
                _binaryStateElement.Deactivate();
            }
        }
    }
}