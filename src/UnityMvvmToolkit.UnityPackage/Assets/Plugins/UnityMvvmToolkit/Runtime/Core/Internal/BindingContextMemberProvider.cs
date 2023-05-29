using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Attributes;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Helpers;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal
{
    internal sealed class BindingContextMemberProvider : IClassMemberProvider
    {
        public void GetBindingContextMembers(Type bindingContextType, IDictionary<int, MemberInfo> result)
        {
            if (typeof(IBindingContext).IsAssignableFrom(bindingContextType) == false)
            {
                throw new InvalidOperationException(
                    $"{bindingContextType.Name} is not assignable from {nameof(IBindingContext)}.");
            }

            var memberInfosSpan = bindingContextType
                .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .AsSpan();

            for (var i = 0; i < memberInfosSpan.Length; i++)
            {
                var memberInfo = memberInfosSpan[i];

                if (TryGetMemberHashCode(bindingContextType, memberInfo, out var hashCode))
                {
                    result.Add(hashCode, memberInfo);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetMemberHashCode(Type contextType, MemberInfo memberInfo, out int hashCode)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return TryGetFieldHashCode(contextType, (FieldInfo) memberInfo, out hashCode);
                case MemberTypes.Property:
                    return TryGetPropertyHashCode(contextType, (PropertyInfo) memberInfo, out hashCode);
                default:
                    hashCode = default;
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetFieldHashCode(Type contextType, FieldInfo fieldInfo, out int hashCode)
        {
            if (fieldInfo.IsDefined(typeof(CompilerGeneratedAttribute), false))
            {
                hashCode = default;
                return false;
            }

            if (HasObservableAttribute(fieldInfo, out var propertyName))
            {
                return string.IsNullOrWhiteSpace(propertyName)
                    ? TryGetHashCode(contextType, GetBindableName(fieldInfo.Name), fieldInfo.FieldType, out hashCode)
                    : TryGetHashCode(contextType, propertyName, fieldInfo.FieldType, out hashCode);
            }

            if (fieldInfo.IsPublic)
            {
                return TryGetHashCode(contextType, fieldInfo.Name, fieldInfo.FieldType, out hashCode);
            }

            hashCode = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetPropertyHashCode(Type contextType, PropertyInfo propertyInfo, out int hashCode)
        {
            if (HasObservableAttribute(propertyInfo, out var propertyName))
            {
                return string.IsNullOrWhiteSpace(propertyName)
                    ? TryGetHashCode(contextType, GetBindableName(propertyInfo.Name), propertyInfo.PropertyType, out hashCode)
                    : TryGetHashCode(contextType, propertyName, propertyInfo.PropertyType, out hashCode);
            }

            if (propertyInfo.GetMethod.IsPublic)
            {
                return TryGetHashCode(contextType, propertyInfo.Name, propertyInfo.PropertyType, out hashCode);
            }

            hashCode = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetHashCode(Type contextType, string memberName, Type memberType, out int hashCode)
        {
            if (typeof(IBaseCommand).IsAssignableFrom(memberType) ||
                typeof(IBaseProperty).IsAssignableFrom(memberType))
            {
                hashCode = HashCodeHelper.GetMemberHashCode(contextType, memberName);
                return true;
            }

            hashCode = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasObservableAttribute(MemberInfo fieldInfo, out string propertyName)
        {
            var observableAttribute = fieldInfo.GetCustomAttribute<ObservableAttribute>();

            if (observableAttribute == null)
            {
                propertyName = default;
                return false;
            }

            propertyName = observableAttribute.PropertyName;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetBindableName(string memberName)
        {
            var resultName = memberName;

            if (resultName.Length > 1)
            {
                if (resultName[0] == '_')
                {
                    resultName = resultName[1..]; // TODO: Get rid of allocation.
                }

                if (resultName[0] == 'm' && resultName[1] == '_')
                {
                    resultName = resultName[2..]; // TODO: Get rid of allocation.
                }
            }

            if (string.IsNullOrEmpty(resultName))
            {
                throw new InvalidOperationException($"Field name '{resultName}' is not supported.");
            }

            return resultName;
        }
    }
}