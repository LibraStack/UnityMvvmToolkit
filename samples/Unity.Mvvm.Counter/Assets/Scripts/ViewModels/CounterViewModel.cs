using Enums;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace ViewModels
{
    public class CounterViewModel : ViewModel
    {
        private int _count;
        private ThemeMode _themeMode;

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

        public ThemeMode ThemeMode
        {
            get => _themeMode;
            set => Set(ref _themeMode, value);
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