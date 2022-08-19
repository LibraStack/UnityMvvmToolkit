using System;
using System.Collections.Generic;using Interfaces;
using Services;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;
using UnityMvvmToolkit.Core.Interfaces;
using ViewModels;
using Views;

public class AppContext : MonoBehaviour, IAppContext, IDisposable
{
    [SerializeField] private MainView _mainView;
    [SerializeField] private AddTaskDialogView _addTaskDialogView;
    
    [Space]
    [SerializeField] private VisualTreeAsset _taskItemAsset;

    private List<IDisposable> _disposables;
    private Dictionary<Type, object> _registeredTypes;

    public void Construct()
    {
        _disposables = new List<IDisposable>();
        _registeredTypes = new Dictionary<Type, object>();

        RegisterInstance(_mainView);
        RegisterInstance(_addTaskDialogView);
        
        RegisterInstance(new TaskBroker());
        RegisterInstance<IDialogsService>(new DialogsService(this));
        
        RegisterInstance(new MainViewModel(this));
        RegisterInstance(new AddTaskDialogViewModel(this));
        
        RegisterInstance<IBindableElementsWrapper>(new ToDoListBindableElementsWrapper(_taskItemAsset));
        RegisterInstance(GetValueConverters());
    }

    public T Resolve<T>()
    {
        return (T) _registeredTypes[typeof(T)];
    }

    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
    }

    private void RegisterInstance<T>(T instance)
    {
        if (instance is IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        _registeredTypes.Add(typeof(T), instance);
    }

    private IValueConverter[] GetValueConverters()
    {
        return new IValueConverter[]
        {
            new IntToStrConverter()
        };
    }
}