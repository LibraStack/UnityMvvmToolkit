using UnityMvvmToolkit.Core.Converters.ParameterValueConverters;

namespace UnityMvvmToolkit.Test.Integration.TestValueConverters;

public class InvertBoolParamConverter : ParameterValueConverter<bool>
{
    public override bool Convert(string parameter)
    {
        bool.TryParse(parameter, out var result);
        return !result;
    }
}