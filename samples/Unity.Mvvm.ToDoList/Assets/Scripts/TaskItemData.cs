using System;
using UnityMvvmToolkit.Common.Interfaces;

public class TaskItemData : ICollectionItemData
{
    public Guid Id { get; } = Guid.NewGuid();

    public string Name { get; set; }
    public bool IsDone { get; set; }
}