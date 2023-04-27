using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Test.Unit.Interfaces;

namespace UnityMvvmToolkit.Test.Unit.TestCommands;

public class MyCommand : Command, IMyCommand
{
    public MyCommand(Action action, Func<bool>? canExecute = null) : base(action, canExecute)
    {
    }
}