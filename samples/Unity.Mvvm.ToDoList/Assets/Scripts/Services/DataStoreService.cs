using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Cysharp.Threading.Tasks;
using Interfaces;
using Interfaces.Services;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityMvvmToolkit.Common.Interfaces;
using ViewModels;

namespace Services
{
    public class DataStoreService : IDataStoreService
    {
        private const string DataFileName = "todolist.json";

        private readonly MainViewModel _mainViewModel;
        private readonly string _dataFilePath;

        private AsyncLazy _saveDataTask;
        private AsyncLazy<IEnumerable<ICollectionItem>> _loadDataTask;

        public DataStoreService(IAppContext appContext)
        {
            _mainViewModel = appContext.Resolve<MainViewModel>();
            _dataFilePath = Path.Combine(Application.persistentDataPath, DataFileName);
        }

        public async void Enable()
        {
            try
            {
                _mainViewModel.AddTasks(await LoadDataAsync());
            }
            finally
            {
                // _mainViewModel.TaskItems.CollectionChanged += OnTaskItemsCollectionChanged;
            }
        }

        public void Disable()
        {
            // _mainViewModel.TaskItems.CollectionChanged -= OnTaskItemsCollectionChanged;
        }

        private void OnTaskItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action is
                NotifyCollectionChangedAction.Add or
                NotifyCollectionChangedAction.Remove or
                NotifyCollectionChangedAction.Replace)
            {
                SaveDataAsync().Forget();
            }
        }

        private async UniTask SaveDataAsync()
        {
            if (_saveDataTask?.Task.Status.IsCompleted() ?? true)
            {
                _saveDataTask = SaveDataAsync(_dataFilePath, _mainViewModel.TaskItems).ToAsyncLazy();
            }

            await _saveDataTask;
        }

        private async UniTask<IEnumerable<ICollectionItem>> LoadDataAsync()
        {
            if (_loadDataTask?.Task.Status.IsCompleted() ?? true)
            {
                _loadDataTask = LoadDataAsync(_dataFilePath).ToAsyncLazy();
            }

            return await _loadDataTask;
        }

        private static async UniTask SaveDataAsync(string filePath, IEnumerable<ICollectionItem> taskItems)
        {
            await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(taskItems));
        }

        private static async UniTask<IEnumerable<ICollectionItem>> LoadDataAsync(string filePath)
        {
            return GetDefaultDataSet();
            // ;
            // return File.Exists(filePath)
            //     ? JsonConvert.DeserializeObject<IEnumerable<TaskItemViewModel>>(await File.ReadAllTextAsync(filePath))
            //     : GetDefaultDataSet();
        }

        private static IEnumerable<ICollectionItem> GetDefaultDataSet()
        {
            return new TaskItemViewModel[]
            {
                new() { Name = "Add UnitTests" },
                new() { Name = "Add UGUI ListView" },
                new() { Name = "Add UI Toolkit ListView", IsDone = true },
                new() { Name = "Provide Custom Binding String Parser" },
                new() { Name = "Publish to OpenUPM" },
                new() { Name = "Fix Command Binding Issue", IsDone = true },
                new() { Name = "Add AsyncCommand", IsDone = true },
                new() { Name = "Add AsyncLazyCommand", IsDone = true },
                new() { Name = "Add Counter Sample", IsDone = true },
                new() { Name = "Add Calculator Sample", IsDone = true },
                new() { Name = "Add ToDoList Sample", IsDone = true }
            };
        }
    }
}