using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace ViewModels
{
    public class CounterViewModel : ViewModel
    {
        private int _count;

        public CounterViewModel()
        {
            IncreaseCommand = new Command(Increase);
        }

        public int Count
        {
            get => _count;
            set => Set(ref _count, value);
        }

        public ICommand IncreaseCommand { get; }

        private void Increase()
        {
            Count++;
        }
    }
}