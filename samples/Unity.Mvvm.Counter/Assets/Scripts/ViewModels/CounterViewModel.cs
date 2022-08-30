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
            IncrementCommand = new Command(IncrementCount);
            DecrementCommand = new Command(DecrementCount);
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

        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }

        private void IncrementCount() => Count++;
        private void DecrementCount() => Count--;
    }
}