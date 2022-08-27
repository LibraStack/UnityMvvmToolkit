using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using ViewModels;

namespace Services
{
    public class DataStoreService : IDataStoreService
    {
        private const string DataFileName = "todolist.xml";

        private readonly MainViewModel _mainViewModel;
        private readonly XmlSerializer _serializer;
        private readonly string _dataFilePath;

        private AsyncLazy _saveDataTask;
        private AsyncLazy<TaskItemData[]> _loadDataTask;

        public DataStoreService(IAppContext appContext)
        {
            _mainViewModel = appContext.Resolve<MainViewModel>();
            _dataFilePath = Path.Combine(Application.persistentDataPath, DataFileName);
            _serializer = new XmlSerializer(typeof(TaskItemData[]));
        }

        public async void Enable()
        {
            try
            {
                _mainViewModel.TaskItems.AddRange(await LoadDataAsync());
            }
            finally
            {
                _mainViewModel.TaskItems.CollectionChanged += OnTaskItemsCollectionChanged;
            }
        }

        public void Disable()
        {
            _mainViewModel.TaskItems.CollectionChanged -= OnTaskItemsCollectionChanged;
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

        private async UniTask<TaskItemData[]> LoadDataAsync()
        {
            if (_loadDataTask?.Task.Status.IsCompleted() ?? true)
            {
                _loadDataTask = LoadDataAsync(_dataFilePath).ToAsyncLazy();
            }

            return await _loadDataTask;
        }

        private async UniTask SaveDataAsync(string filePath, IEnumerable<TaskItemData> taskItems)
        {
            await using var writeStream = File.Create(filePath);
            _serializer.Serialize(writeStream, taskItems.ToArray());
        }

        private async UniTask<TaskItemData[]> LoadDataAsync(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                return GetDefaultDataSet();
            }

            await using var readStream = File.OpenRead(filePath);
            return (TaskItemData[]) _serializer.Deserialize(readStream);
        }

        private TaskItemData[] GetDefaultDataSet()
        {
            return new TaskItemData[]
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