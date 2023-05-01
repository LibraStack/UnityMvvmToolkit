using System.Collections;

namespace UnityMvvmToolkit.Test.Unit.TestDataSets;

public class PropertyNotValidBindingStringDataSet : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return new object?[] { "Count,, IntToStrConverter" };
        yield return new object?[] { "Count, IntToStrConverter,," };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}