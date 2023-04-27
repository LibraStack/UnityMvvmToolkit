using System;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ViewModels
{
    public class TaskItemViewModel : ICollectionItem
    {
        [Observable(nameof(Name))]
        private readonly IProperty<string> _name;

        [Observable(nameof(IsDone))]
        private readonly IProperty<bool> _isDone;

        public TaskItemViewModel()
        {
            _name = new Property<string>();

            _isDone = new Property<bool>();
            _isDone.ValueChanged += OnIsDoneValueChanged;

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

        private void Remove()
        {
            RemoveClick?.Invoke(this, EventArgs.Empty);
        }

        private void OnIsDoneValueChanged(object sender, bool newValue)
        {
            IsDoneChanged?.Invoke(this, newValue);
        }
    }
}