using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace ViewModels
{
    public class CounterViewModel : IBindingContext
    {
        private readonly IProperty<int> _count = new Property<int>();

        public CounterViewModel()
        {
            IncreaseCommand = new Command(Increase);
        }

        public int Count
        {
            get => _count.Value;
            set => _count.Value = value;
        }

        public ICommand IncreaseCommand { get; }

        private void Increase()
        {
            Count++;
        }
    }
}