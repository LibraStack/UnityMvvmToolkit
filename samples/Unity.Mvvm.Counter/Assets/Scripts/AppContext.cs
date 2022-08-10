using System;
using System.Collections.Generic;
using Interfaces;
using Interfaces.Services;
using Services;
using UnityEngine;
using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;
using UnityMvvmToolkit.Core.Interfaces;
using ValueConverters;
using ViewModels;

public class AppContext : MonoBehaviour, IAppContext
{
    [SerializeField] private ThemeService _themeService;

    private Dictionary<Type, object> _registeredTypes;

    public void Construct()
    {
        _registeredTypes = new Dictionary<Type, object>();

        RegisterInstance<IThemeService>(_themeService);
        RegisterInstance(new CounterViewModel());
        RegisterInstance<IDataStoreService>(new DataStoreService(this));
        RegisterInstance<IBindableElementsWrapper>(new CounterBindableElementsWrapper());
        RegisterInstance(GetValueConverters());
    }

    public T Resolve<T>()
    {
        return (T) _registeredTypes[typeof(T)];
    }

    private void RegisterInstance<T>(T instance)
    {
        _registeredTypes.Add(typeof(T), instance);
    }

    private IValueConverter[] GetValueConverters()
    {
        return new IValueConverter[]
        {
            new IntToStrConverter(),
            new ThemeModeToBoolConverter()
        };
    }
}