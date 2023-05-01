using System.Collections;

namespace UnityMvvmToolkit.Test.Unit.TestDataSets;

public class CommandNotValidBindingStringDataSet : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return new object?[] { "ClickCommand,, ParameterToIntConverter" };
        yield return new object?[] { "ClickCommand, ParameterToIntConverter,," };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}