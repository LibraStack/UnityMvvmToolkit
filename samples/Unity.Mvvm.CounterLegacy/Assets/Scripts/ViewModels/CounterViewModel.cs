using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ViewModels
{
    public class CounterViewModel : IBindingContext
    {
        public CounterViewModel()
        {
            Count = new Property<int>();

            IncrementCommand = new Command(IncrementCount);
        }

        public IProperty<int> Count { get; }

        public ICommand IncrementCommand { get; }

        private void IncrementCount() => Count.Value++;
    }
}