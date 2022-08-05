using System;
using BindableUIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableUIElementWrappers
{
    public class BindableCounterSliderWrapper : BindableCommandElement, IDisposable
    {
        private readonly BindableCounterSlider _counterSlider;
        private readonly ICommand _increaseCommand;
        private readonly ICommand _decreaseCommand;

        public BindableCounterSliderWrapper(BindableCounterSlider counterSlider, IObjectProvider objectProvider)
            : base(objectProvider)
        {
            _counterSlider = counterSlider;
            _increaseCommand = GetCommand<ICommand>(counterSlider.IncreaseCommand);
            _decreaseCommand = GetCommand<ICommand>(counterSlider.DecreaseCommand);

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
            _increaseCommand.Execute();
        }

        private void OnDecrease(object sender, EventArgs e)
        {
            _decreaseCommand.Execute();
        }
    }
}