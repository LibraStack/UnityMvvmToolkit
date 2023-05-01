using System;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IParameterValueConverter : IValueConverter
    {
        Type TargetType { get; }
    }
}