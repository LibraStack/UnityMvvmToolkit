using Enums;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;

namespace ViewModels
{
    public class CounterViewModel : ViewModel
    {
        private int _count;
        private bool _isDarkMode;
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

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (Set(ref _isDarkMode, value))
                {
                    OnPropertyChanged(nameof(ThemeMode));
                }
            }
        }

        public ThemeMode ThemeMode => _isDarkMode ? ThemeMode.Dark : ThemeMode.Light;

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