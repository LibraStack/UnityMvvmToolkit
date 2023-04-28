using System.Collections;

namespace UnityMvvmToolkit.Test.Unit.TestData;

public class CommandValidBindingStringTestData : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return new object?[] { "ClickCommand", "ClickCommand", null, null };
        yield return new object?[] { "ClickCommand, 55", "ClickCommand", "55", null };

        yield return new object?[]
        {
            "ClickCommand, 55, ParameterToIntConverter", "ClickCommand", "55", "ParameterToIntConverter"
        };

        yield return new object?[]
        {
            "ClickCommand, Parameter={55}, Converter={ParameterConverter}", "ClickCommand", "55", "ParameterConverter"
        };

        yield return new object?[]
        {
            "ClickCommand, Converter={ParameterConverter}, Parameter={55}", "ClickCommand", "55", "ParameterConverter"
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}