using System;
using UnityMvvmToolkit.Common.Interfaces;

public class TaskItemData : ICollectionItemData
{
    public TaskItemData(string name)
    {
        Name = name;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public string Name { get; }
    public bool IsDone { get; set; }
}