using Enums;
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

            _counterViewModel.Count.ValueChanged += OnCountValueChanged;
            _counterViewModel.ThemeMode.ValueChanged += OnThemeModeValueChanged;
        }

        public void Disable()
        {
            _counterViewModel.Count.ValueChanged += OnCountValueChanged;
            _counterViewModel.ThemeMode.ValueChanged += OnThemeModeValueChanged;
        }

        private void OnCountValueChanged(object sender, int newValue)
        {
            PlayerPrefs.SetInt(nameof(_counterViewModel.Count), newValue);
            PlayerPrefs.Save();
        }

        private void OnThemeModeValueChanged(object sender, ThemeMode newValue)
        {
            PlayerPrefs.SetInt(nameof(_counterViewModel.ThemeMode), (int) newValue);
            PlayerPrefs.Save();
        }

        private void LoadData()
        {
            _counterViewModel.Count.Value = PlayerPrefs.GetInt(nameof(_counterViewModel.Count));
            _counterViewModel.ThemeMode.Value = (ThemeMode) PlayerPrefs.GetInt(nameof(_counterViewModel.ThemeMode));
        }
    }
}