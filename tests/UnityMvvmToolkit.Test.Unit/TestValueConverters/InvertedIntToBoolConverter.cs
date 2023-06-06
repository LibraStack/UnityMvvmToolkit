using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;

namespace UnityMvvmToolkit.Test.Unit.TestValueConverters;

public class InvertedIntToBoolConverter : PropertyValueConverter<int, bool>
{
    public override bool Convert(int value)
    {
        return value == 0;
    }

    public override int ConvertBack(bool value)
    {
        return value ? 0 : 1;
    }
}