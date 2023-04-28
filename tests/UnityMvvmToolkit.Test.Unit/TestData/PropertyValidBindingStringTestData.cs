using System.Collections;

namespace UnityMvvmToolkit.Test.Unit.TestData;

public class PropertyValidBindingStringTestData : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return new object?[] { "Count", "Count", null };
        yield return new object?[] { "Count, IntToStrConverter", "Count", "IntToStrConverter" };
        yield return new object?[] { "Count, Converter={IntToStrConverter}", "Count", "IntToStrConverter" };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}