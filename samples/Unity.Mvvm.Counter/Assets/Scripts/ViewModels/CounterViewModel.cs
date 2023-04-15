using Enums;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace ViewModels
{
    public class CounterViewModel : IBindingContext
    {
        private readonly IProperty<int> _count = new ObservableProperty<int>();
        private readonly IProperty<ThemeMode> _themeMode = new ObservableProperty<ThemeMode>();

        public CounterViewModel()
        {
            IncrementCommand = new Command(IncrementCount);
            DecrementCommand = new Command(DecrementCount);
        }

        public int Count
        {
            get => _count.Value;
            set => _count.Value = value;
        }

        public ThemeMode ThemeMode
        {
            get => _themeMode.Value;
            set => _themeMode.Value = value;
        }

        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }

        private void IncrementCount() => Count++;
        private void DecrementCount() => Count--;
    }
}