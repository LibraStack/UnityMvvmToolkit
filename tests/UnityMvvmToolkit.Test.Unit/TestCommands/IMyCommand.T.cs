using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Test.Unit.TestCommands;

public interface IMyCommand<in T> : ICommand<T>
{
}