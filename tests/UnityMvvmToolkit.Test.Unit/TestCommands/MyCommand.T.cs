using UnityMvvmToolkit.Core;

namespace UnityMvvmToolkit.Test.Unit.TestCommands;

public class MyCommand<T> : Command<T>, IMyCommand<T>
{
    public MyCommand(Action<T>? action, Func<bool>? canExecute = null) : base(action, canExecute)
    {
    }
}