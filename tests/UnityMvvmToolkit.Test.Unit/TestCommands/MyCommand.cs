using UnityMvvmToolkit.Core;

namespace UnityMvvmToolkit.Test.Unit.TestCommands;

public class MyCommand : Command, IMyCommand
{
    public MyCommand(Action? action, Func<bool>? canExecute = null) : base(action, canExecute)
    {
    }
}