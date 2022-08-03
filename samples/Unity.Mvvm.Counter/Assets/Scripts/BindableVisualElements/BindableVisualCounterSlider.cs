using System;
using BindableUIElements;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableVisualElements
{
    public class BindableVisualCounterSlider : IBindableElement, IDisposable
    {
        private readonly BindableCounterSlider _counterSlider;
        private readonly ICommand _increaseCommand;
        private readonly ICommand _decreaseCommand;

        public BindableVisualCounterSlider(BindableCounterSlider counterSlider, IPropertyProvider propertyProvider)
        {
            _counterSlider = counterSlider;
            _increaseCommand = propertyProvider.GetReadOnlyProperty<ICommand>(counterSlider.IncreaseCommand)?.Value;
            _decreaseCommand = propertyProvider.GetReadOnlyProperty<ICommand>(counterSlider.DecreaseCommand)?.Value;

            if (_increaseCommand != null)
            {
                _counterSlider.Increase += OnIncrease;
            }

            if (_decreaseCommand != null)
            {
                _counterSlider.Decrease += OnDecrease;
            }
        }

        public void Dispose()
        {
            if (_increaseCommand != null)
            {
                _counterSlider.Increase -= OnIncrease;
            }

            if (_decreaseCommand != null)
            {
                _counterSlider.Decrease -= OnDecrease;
            }
        }

        private void OnIncrease(object sender, EventArgs e)
        {
            _increaseCommand.Execute(null);
        }
        
        private void OnDecrease(object sender, EventArgs e)
        {
            _decreaseCommand.Execute(null);
        }
    }
}