using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.Extensions
{
    internal static class MemberInfoExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetMemberValue<T>(this MemberInfo memberInfo, IBindingContext context, out Type memberType)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                {
                    var fieldInfo = (FieldInfo) memberInfo;
                    memberType = fieldInfo.FieldType;

                    return (T) fieldInfo.GetValue(context);
                }

                case MemberTypes.Property:
                {
                    var propertyInfo = (PropertyInfo) memberInfo;
                    memberType = propertyInfo.PropertyType;

                    return (T) propertyInfo.GetValue(context);
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}