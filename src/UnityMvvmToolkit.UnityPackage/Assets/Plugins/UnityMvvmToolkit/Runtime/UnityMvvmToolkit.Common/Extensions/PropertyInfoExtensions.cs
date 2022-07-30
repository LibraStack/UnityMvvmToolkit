using System;
using System.Reflection;

namespace UnityMvvmToolkit.Common.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static bool HasSetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.SetMethod != null;
        }

        public static Func<TObjectType, TValueType> CreateGetValueDelegate<TObjectType, TValueType>(
            this PropertyInfo propertyInfo)
        {
            return (Func<TObjectType, TValueType>) Delegate.CreateDelegate(typeof(Func<TObjectType, TValueType>),
                propertyInfo.GetMethod);
        }

        public static Action<TObjectType, TValueType> CreateSetValueDelegate<TObjectType, TValueType>(
            this PropertyInfo propertyInfo)
        {
            return (Action<TObjectType, TValueType>) Delegate.CreateDelegate(typeof(Action<TObjectType, TValueType>),
                propertyInfo.SetMethod);
        }
    }
}