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
            UpdateCountCommand = new Command(UpdateCount);
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

        public ICommand UpdateCountCommand { get; }

        private void UpdateCount(string parameter) // TODO: Implement via IncreaseCommand & DecreaseCommand.
        {
            if (parameter == "+")
            {
                Count++;
            }
            else
            {
                Count--;
            }
        }
    }
}