using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;

namespace ViewModels
{
    public class CounterViewModel : ViewModel
    {
        private int _count;
        private bool _isDarkMode;

        public CounterViewModel()
        {
            IncreaseCommand = new Command(IncreaseCount);
            DecreaseCommand = new Command(DecreaseCount);
        }

        public int Count
        {
            get => _count;
            set => Set(ref _count, value);
        }

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set => Set(ref _isDarkMode, value);
        }

        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }

        private void IncreaseCount()
        {
            Count++;
        }

        private void DecreaseCount()
        {
            Count--;
        }
    }
}