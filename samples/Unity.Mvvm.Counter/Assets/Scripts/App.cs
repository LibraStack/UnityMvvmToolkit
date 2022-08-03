using System.ComponentModel;
using Enums;
using Interfaces.Services;
using UnityEngine;
using ViewModels;

[DefaultExecutionOrder(-1)]
public class App : MonoBehaviour
{
    [SerializeField] private AppContext _appContext;

    private IThemeService _themeService;
    private CounterViewModel _counterViewModel;
    private IDataStoreService _dataStoreService;

    private void Awake()
    {
        _appContext.Construct();

        _themeService = _appContext.Resolve<IThemeService>();
        _counterViewModel = _appContext.Resolve<CounterViewModel>();
        _dataStoreService = _appContext.Resolve<IDataStoreService>();
    }

    private void Start()
    {
        Application.targetFrameRate = 300;

        _counterViewModel.PropertyChanged += OnCounterViewModelPropertyChanged;
        _dataStoreService.Enable();
    }

    private void OnDestroy()
    {
        _counterViewModel.PropertyChanged -= OnCounterViewModelPropertyChanged;
        _dataStoreService.Disable();
    }

    private void OnCounterViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_counterViewModel.IsDarkMode)) // TODO: Bind a view directly to the property?
        {
            _themeService.SetThemeMode(_counterViewModel.IsDarkMode ? ThemeMode.Dark : ThemeMode.Light);
        }
    }
}