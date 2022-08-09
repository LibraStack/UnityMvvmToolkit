using System;
using BindableUIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElementWrappers
{
    public class BindableCounterSliderWrapper : BindableCommandElement, IInitializable, IDisposable
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
        }

        public bool CanInitialize => _increaseCommand != null && _decreaseCommand != null;

        public void Initialize()
        {
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
            _increaseCommand.Execute();
        }

        private void OnDecrease(object sender, EventArgs e)
        {
            _decreaseCommand.Execute();
        }
    }
}