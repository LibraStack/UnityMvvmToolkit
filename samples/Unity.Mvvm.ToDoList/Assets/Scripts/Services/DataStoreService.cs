using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
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
        private AsyncLazy<Collection<TaskItemData>> _loadDataTask;

        public DataStoreService(IAppContext appContext)
        {
            _mainViewModel = appContext.Resolve<MainViewModel>();
            _dataFilePath = Path.Combine(Application.persistentDataPath, DataFileName);
            _serializer = new XmlSerializer(typeof(Collection<TaskItemData>));
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

        private async UniTask<Collection<TaskItemData>> LoadDataAsync()
        {
            if (_loadDataTask?.Task.Status.IsCompleted() ?? true)
            {
                _loadDataTask = LoadDataAsync(_dataFilePath).ToAsyncLazy();
            }

            return await _loadDataTask;
        }

        private async UniTask SaveDataAsync(string filePath, Collection<TaskItemData> taskItems)
        {
            await using var writeStream = File.Create(filePath);
            _serializer.Serialize(writeStream, taskItems);
        }

        private async UniTask<Collection<TaskItemData>> LoadDataAsync(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                return GetDefaultDataSet();
            }

            await using var readStream = File.OpenRead(filePath);
            return (Collection<TaskItemData>) _serializer.Deserialize(readStream);
        }

        private Collection<TaskItemData> GetDefaultDataSet()
        {
            return new Collection<TaskItemData>
            {
                new() { Name = "Add UnitTests" },
                new() { Name = "Create UGUI ListView" },
                new() { Name = "Write an article" },
                new() { Name = "Add UI Toolkit ListView" },
                new() { Name = "Fix Command bindings" }
            };
        }
    }
}