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

            if (fieldInfo.IsPublic)
            {
                return TryGetHashCode(contextType, fieldInfo.Name, fieldInfo.FieldType.GetInterfaces(), out hashCode);
            }

            if (TryGetPropertyNameFromAttribute(fieldInfo, out var fieldName))
            {
                return TryGetHashCode(contextType, fieldName, fieldInfo.FieldType.GetInterfaces(), out hashCode);
            }

            fieldName = fieldInfo.Name;

            if (fieldName.Length > 1)
            {
                if (fieldName[0] == '_')
                {
                    fieldName = fieldName[1..]; // TODO: Get rid of allocation.
                }

                if (fieldName[0] == 'm' && fieldName[1] == '_')
                {
                    fieldName = fieldName[2..]; // TODO: Get rid of allocation.
                }
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new InvalidOperationException($"Field name '{fieldName}' is not supported.");
            }

            return TryGetHashCode(contextType, fieldName, fieldInfo.FieldType.GetInterfaces(), out hashCode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetPropertyNameFromAttribute(MemberInfo fieldInfo, out string propertyName)
        {
            var observableAttribute = fieldInfo.GetCustomAttribute<ObservableAttribute>();

            if (observableAttribute == null || string.IsNullOrWhiteSpace(observableAttribute.PropertyName))
            {
                propertyName = default;
                return false;
            }

            propertyName = observableAttribute.PropertyName;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetPropertyHashCode(Type contextType, PropertyInfo propertyInfo, out int hashCode)
        {
            return TryGetHashCode(contextType, propertyInfo.Name, propertyInfo.PropertyType.GetInterfaces(),
                out hashCode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetHashCode(Type contextType, string memberName, Type[] memberInterfaces,
            out int hashCode)
        {
            for (var i = 0; i < memberInterfaces.Length; i++)
            {
                var interfaceType = memberInterfaces[i];

                if (interfaceType == typeof(IBaseCommand) ||
                    interfaceType == typeof(IBaseProperty))
                {
                    hashCode = HashCodeHelper.GetMemberHashCode(contextType, memberName);
                    return true;
                }
            }

            hashCode = default;
            return false;
        }
    }
}