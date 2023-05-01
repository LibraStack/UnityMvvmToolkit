using System;

namespace UnityMvvmToolkit.Core.Interfaces
{
    public interface IPropertyValueConverter : IValueConverter
    {
        Type SourceType { get; }
        Type TargetType { get; }
    }
}