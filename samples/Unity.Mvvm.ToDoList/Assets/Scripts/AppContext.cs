using System;
using System.Collections.Generic;using Interfaces;
using Interfaces.Services;
using Services;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;
using UnityMvvmToolkit.Core.Interfaces;
using ViewModels;
using Views;

public class AppContext : MonoBehaviour, IAppContext, IDisposable
{
    [SerializeField] private VisualTreeAsset _taskItemAsset;
    [SerializeField] private AddTaskDialogView _addTaskView;

    private List<IDisposable> _disposables;
    private Dictionary<Type, object> _registeredTypes;

    public void Construct()
    {
        _disposables = new List<IDisposable>();
        _registeredTypes = new Dictionary<Type, object>();

        RegisterInstance(_addTaskView);

        RegisterInstance(new TaskBroker());
        RegisterInstance<IDialogsService>(new DialogsService(this));

        RegisterInstance(new MainViewModel(this));
        RegisterInstance(new AddTaskDialogViewModel(this));

        RegisterInstance<IDataStoreService>(new DataStoreService(this));
        RegisterInstance(GetValueConverters());
        RegisterInstance(GetCollectionItemTemplates());
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
        if (instance is IInitializable initializable)
        {
            initializable.Initialize();
        }

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

    private IReadOnlyDictionary<Type, object> GetCollectionItemTemplates()
    {
        return new Dictionary<Type, object>
        {
            { typeof(TaskItemViewModel), _taskItemAsset }
        };
    }
}