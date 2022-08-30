using System;
using BindableUIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElementWrappers
{
    public class BindableCounterSliderWrapper : BindableCommandElement, IInitializable, IDisposable
    {
        private readonly BindableCounterSlider _counterSlider;
        private readonly ICommand _incrementCommand;
        private readonly ICommand _decrementCommand;

        public BindableCounterSliderWrapper(BindableCounterSlider counterSlider, IObjectProvider objectProvider)
            : base(objectProvider)
        {
            _counterSlider = counterSlider;
            _incrementCommand = GetCommand<ICommand>(counterSlider.IncrementCommand);
            _decrementCommand = GetCommand<ICommand>(counterSlider.DecrementCommand);
        }

        public bool CanInitialize => _incrementCommand != null && _decrementCommand != null;

        public void Initialize()
        {
            _counterSlider.Increment += OnIncrement;
            _counterSlider.Decrement += OnDecrement;
        }

        public void Dispose()
        {
            _counterSlider.Increment -= OnIncrement;
            _counterSlider.Decrement -= OnDecrement;
        }

        private void OnIncrement(object sender, EventArgs e)
        {
            _incrementCommand.Execute();
        }

        private void OnDecrement(object sender, EventArgs e)
        {
            _decrementCommand.Execute();
        }
    }
}