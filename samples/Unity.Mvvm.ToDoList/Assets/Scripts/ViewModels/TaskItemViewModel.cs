using System;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace ViewModels
{
    public class TaskItemViewModel : ICollectionItem, IInitializable, IDisposable
    {
        private readonly IProperty<bool> _isDone = new Property<bool>();
        private readonly IProperty<string> _name = new Property<string>();

        public TaskItemViewModel()
        {
            Id = Guid.NewGuid().GetHashCode();
            RemoveCommand = new Command(Remove);
        }

        public int Id { get; }

        public string Name
        {
            get => _name.Value;
            set => _name.Value = value;
        }

        public bool IsDone
        {
            get => _isDone.Value;
            set => _isDone.Value = value;
        }

        public ICommand RemoveCommand { get; }

        public event EventHandler RemoveClick;
        public event EventHandler<bool> IsDoneChanged;

        public void Initialize()
        {
            _isDone.ValueChanged += OnIsDoneValueChanged;
        }

        public void Dispose()
        {
            _isDone.ValueChanged -= OnIsDoneValueChanged;
        }

        private void OnIsDoneValueChanged(object sender, bool newValue)
        {
            IsDoneChanged?.Invoke(this, newValue);
        }

        private void Remove()
        {
            RemoveClick?.Invoke(this, EventArgs.Empty);
        }
    }
}