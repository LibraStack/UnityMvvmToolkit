using System.ComponentModel;
using Interfaces;
using Interfaces.Services;
using UnityEngine;
using ViewModels;

namespace Services
{
    public class DataStoreService : IDataStoreService
    {
        private readonly CounterViewModel _counterViewModel;

        public DataStoreService(IAppContext appContext)
        {
            _counterViewModel = appContext.Resolve<CounterViewModel>();
        }

        public void Enable()
        {
            LoadData();

            _counterViewModel.PropertyChanged += OnCounterViewModelPropertyChanged;
        }

        public void Disable()
        {
            _counterViewModel.PropertyChanged -= OnCounterViewModelPropertyChanged;
        }

        private void OnCounterViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_counterViewModel.Count):
                    PlayerPrefs.SetInt(e.PropertyName, _counterViewModel.Count);
                    break;

                case nameof(_counterViewModel.IsDarkMode):
                    PlayerPrefs.SetInt(e.PropertyName, _counterViewModel.IsDarkMode ? 1 : 0);
                    break;
            }

            PlayerPrefs.Save();
        }

        private void LoadData()
        {
            _counterViewModel.Count = PlayerPrefs.GetInt(nameof(_counterViewModel.Count));
            _counterViewModel.IsDarkMode = PlayerPrefs.GetInt(nameof(_counterViewModel.IsDarkMode)) == 1;
        }
    }
}