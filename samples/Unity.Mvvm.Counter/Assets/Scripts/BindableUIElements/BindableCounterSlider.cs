using System;
using UIElements;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElements
{
    public partial class BindableCounterSlider : CounterSlider, IBindableElement
    {
        private ICommand _incrementCommand;
        private ICommand _decrementCommand;

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _incrementCommand = objectProvider.GetCommand<ICommand>(context, IncrementCommand);
            _decrementCommand = objectProvider.GetCommand<ICommand>(context, DecrementCommand);

            Increment += OnIncrement;
            Decrement += OnDecrement;
        }

        public void ResetBindingContext(IObjectProvider objectProvider)
        {
            Increment -= OnIncrement;
            Decrement -= OnDecrement;

            _incrementCommand = null;
            _decrementCommand = null;
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