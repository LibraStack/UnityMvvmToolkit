using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Helpers;

namespace UnityMvvmToolkit.Core.Internal.ObjectProviders
{
    internal class BindingContextMembersProvider
    {
        public void GetBindingContextMembers(Type bindingContextType, Dictionary<int, MemberInfo> result)
        {
            var memberInfosSpan = bindingContextType
                .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .AsSpan();

            for (var i = 0; i < memberInfosSpan.Length; i++)
            {
                var memberInfo = memberInfosSpan[i];

                if (TryGetMemberHashCode(bindingContextType, memberInfo, out var hashCode))
                {
                    result.TryAdd(hashCode, memberInfo);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetMemberHashCode(Type contextType, MemberInfo memberInfo, out int hashCode)
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
        private bool TryGetFieldHashCode(Type contextType, FieldInfo fieldInfo, out int hashCode)
        {
            if (fieldInfo.IsDefined(typeof(CompilerGeneratedAttribute), false))
            {
                hashCode = default;
                return false;
            }

            var fieldName = fieldInfo.Name;

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

            return TryGetHashCode(contextType, fieldName, fieldInfo.FieldType.GetInterfaces(), out hashCode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetPropertyHashCode(Type contextType, PropertyInfo propertyInfo, out int hashCode)
        {
            return TryGetHashCode(contextType, propertyInfo.Name, propertyInfo.PropertyType.GetInterfaces(),
                out hashCode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetHashCode(Type contextType, string memberName, Type[] memberInterfaces, out int hashCode)
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