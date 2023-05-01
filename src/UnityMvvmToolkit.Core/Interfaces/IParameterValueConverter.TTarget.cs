using System.Runtime.CompilerServices;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IParameterValueConverter<out TTargetType> : IParameterValueConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TTargetType Convert(string parameter);
    }
}