using System;
using UnityMvvmToolkit.Core.Internal.Helpers;

namespace UnityMvvmToolkit.Core.Internal.Interfaces
{
    internal interface IPropertyWrapper
    {
        IPropertyWrapper SetProperty(object property);
        void Reset();

        static int GenerateHashCode(Type valueType, Type sourceType)
        {
            var valueTypeHash = valueType.GetHashCode();
            var sourceTypeHash = sourceType.GetHashCode();

            return HashCodeHelper.CombineHashCode(valueTypeHash, sourceTypeHash);
        }
    }
}