using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.UnitTests.Interfaces;

namespace UnityMvvmToolkit.UnitTests.TestCommands;

public class MyCommand : Command, IMyCommand
{
    public MyCommand(Action action, Func<bool>? canExecute = null) : base(action, canExecute)
    {
    }
}