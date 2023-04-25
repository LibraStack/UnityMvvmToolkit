using Enums;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace ViewModels
{
    public class CounterViewModel : IBindingContext
    {
        public CounterViewModel()
        {
            Count = new Property<int>();
            ThemeMode = new Property<ThemeMode>();

            IncrementCommand = new Command(IncrementCount);
            DecrementCommand = new Command(DecrementCount);
        }

        public IProperty<int> Count { get; }
        public IProperty<ThemeMode> ThemeMode { get; }

        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }

        private void IncrementCount() => Count.Value++;
        private void DecrementCount() => Count.Value--;
    }
}