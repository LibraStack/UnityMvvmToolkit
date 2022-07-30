using System;
using BindableUIElements;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableVisualElements
{
    public class BindableVisualCounterSlider : IBindableElement, IDisposable
    {
        private readonly BindableCounterSlider _counterSlider;
        private readonly ICommand _command;
        private readonly string _increaseCommandParameter;
        private readonly string _decreaseCommandParameter;

        public BindableVisualCounterSlider(BindableCounterSlider counterSlider, IReadOnlyProperty<ICommand> property)
        {
            _counterSlider = counterSlider;
            _command = property.Value;
            _increaseCommandParameter = counterSlider.IncreaseCommandParameter;
            _decreaseCommandParameter = counterSlider.DecreaseCommandParameter;

            _counterSlider.Increase += OnIncrease;
            _counterSlider.Decrease += OnDecrease;
        }

        public void Dispose()
        {
            _counterSlider.Increase -= OnIncrease;
            _counterSlider.Decrease -= OnDecrease;
        }

        private void OnIncrease(object sender, EventArgs e)
        {
            _command.Execute(_increaseCommandParameter);
        }
        
        private void OnDecrease(object sender, EventArgs e)
        {
            _command.Execute(_decreaseCommandParameter);
        }
    }
}