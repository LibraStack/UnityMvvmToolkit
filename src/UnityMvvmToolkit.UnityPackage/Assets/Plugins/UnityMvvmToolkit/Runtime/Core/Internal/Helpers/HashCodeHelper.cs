using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.Helpers
{
    internal static class HashCodeHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetMemberHashCode(Type contextType, string memberName)
        {
            var contextTypeHash = contextType.GetHashCode();
            var memberNameHash = StringComparer.OrdinalIgnoreCase.GetHashCode(memberName);

            return CombineHashCode(contextTypeHash, memberNameHash);
        }

        /// <summary>
        /// <para><c>If name is null:</c></para>
        /// <b>TargetTypeHash + SourceTypeHash</b>
        /// <para><c>Else:</c></para>
        /// <b>NameHash + TargetTypeHash + SourceTypeHash</b>
        /// </summary>
        /// <param name="converter">Converter.</param>
        /// <param name="converterName">Converter name.</param>
        /// <returns>Hash code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPropertyConverterHashCode(IPropertyValueConverter converter, string converterName = null)
        {
            var targetTypeHash = converter.TargetType.GetHashCode();
            var sourceTypeHash = converter.SourceType.GetHashCode();

            return string.IsNullOrWhiteSpace(converterName)
                ? CombineHashCode(targetTypeHash, sourceTypeHash)
                : CombineHashCode(converterName.GetHashCode(), targetTypeHash, sourceTypeHash);
        }

        /// <summary>
        /// <para><c>If name is null:</c></para>
        /// <b>TargetTypeHash</b>
        /// <para><c>Else:</c></para>
        /// <b>NameHash + TargetTypeHash</b>
        /// </summary>
        /// <param name="converter">Converter.</param>
        /// <param name="converterName">Converter name.</param>
        /// <returns>Hash code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetParameterConverterHashCode(IParameterValueConverter converter, string converterName = null)
        {
            var targetTypeHash = converter.TargetType.GetHashCode();

            return string.IsNullOrWhiteSpace(converterName)
                ? targetTypeHash
                : CombineHashCode(converterName.GetHashCode(), targetTypeHash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPropertyWrapperConverterId(IPropertyValueConverter converter, string converterName = null)
        {
            return GetPropertyConverterHashCode(converter, converterName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPropertyWrapperConverterId(Type targetType, Type sourceType, string converterName = null)
        {
            return string.IsNullOrWhiteSpace(converterName)
                ? CombineHashCode(targetType.GetHashCode(), sourceType.GetHashCode())
                : CombineHashCode(converterName.GetHashCode(), targetType.GetHashCode(), sourceType.GetHashCode());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCommandWrapperConverterId(IParameterValueConverter converter, string converterName = null)
        {
            return GetParameterConverterHashCode(converter, converterName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCommandWrapperConverterId(Type targetType, string converterName = null)
        {
            return string.IsNullOrWhiteSpace(converterName)
                ? targetType.GetHashCode()
                : CombineHashCode(converterName.GetHashCode(), targetType.GetHashCode());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCommandWrapperId(Type contextType, Type targetType, string converterName = null)
        {
            return string.IsNullOrWhiteSpace(converterName)
                ? CombineHashCode(contextType.GetHashCode(), targetType.GetHashCode())
                : CombineHashCode(contextType.GetHashCode(), converterName.GetHashCode(), targetType.GetHashCode());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCode(int hash1, int hash2)
        {
            var hash = 17;
            hash = hash * 31 + hash1;
            hash = hash * 31 + hash2;

            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCode(int hash1, int hash2, int hash3)
        {
            return CombineHashCode(CombineHashCode(hash1, hash2), hash3);
        }
    }
}