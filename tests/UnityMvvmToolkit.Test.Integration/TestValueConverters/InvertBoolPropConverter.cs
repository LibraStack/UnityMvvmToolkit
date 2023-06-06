using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;

namespace UnityMvvmToolkit.Test.Integration.TestValueConverters;

public class InvertBoolPropConverter : PropertyValueConverter<bool, bool>
{
    public override bool Convert(bool value)
    {
        return !value;
    }

    public override bool ConvertBack(bool value)
    {
        throw new NotImplementedException();
    }
}