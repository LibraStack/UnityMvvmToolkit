using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;

namespace UnityMvvmToolkit.Test.Unit.TestValueConverters;

public class IntToBoolConverter : PropertyValueConverter<int, bool>
{
    public override bool Convert(int value)
    {
        return value == 1;
    }

    public override int ConvertBack(bool value)
    {
        return value ? 1 : 0;
    }
}