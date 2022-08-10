using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.PropertyWrappers;

namespace UnityMvvmToolkit.Core.Internal.ObjectProviders
{
    internal class PropertyProvider<TBindingContext> : ObjectProvider<TBindingContext>
    {
        private readonly HashSet<IValueConverter> _valueConverters;

        internal PropertyProvider(TBindingContext bindingContext, IEnumerable<IConverter> converters)
            : base(bindingContext)
        {
            _valueConverters = GetConverters<IValueConverter>(converters);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IProperty<TValueType> GetProperty<TValueType>(string propertyName, ReadOnlyMemory<char> converterName)
        {
            return CreateProperty<IProperty<TValueType>, TValueType>(propertyName, converterName,
                typeof(PropertyWrapper<,>), typeof(PropertyWrapperWithConverter<,,>));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyProperty<TValueType> GetReadOnlyProperty<TValueType>(string propertyName,
            ReadOnlyMemory<char> converterName)
        {
            return CreateProperty<IReadOnlyProperty<TValueType>, TValueType>(propertyName, converterName,
                typeof(ReadOnlyPropertyWrapper<,>), typeof(ReadOnlyPropertyWrapperWithConverter<,,>));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TProperty CreateProperty<TProperty, TValueType>(string propertyName, ReadOnlyMemory<char> converterName,
            Type propertyType, Type propertyWithValueConverterType)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return default;
            }

            if (typeof(TValueType).GetInterface(nameof(IBaseCommand)) != null)
            {
                throw new InvalidOperationException(
                    $"To get a command use the {nameof(CommandProvider<TBindingContext>.GetCommand)} method instead.");
            }

            if (TryGetInstanceFromCache<TProperty>(propertyName, out var property))
            {
                return property;
            }

            AssurePropertyExist(propertyName, out var propertyInfo);

            object[] args;
            Type genericPropertyType;

            if (typeof(TValueType) == propertyInfo.PropertyType)
            {
                args = new object[]
                {
                    BindingContext, propertyInfo
                };

                genericPropertyType = propertyType.MakeGenericType(typeof(TBindingContext), typeof(TValueType));
            }
            else
            {
                args = new object[]
                {
                    BindingContext, propertyInfo,
                    GetValueConverter<TValueType>(propertyInfo.PropertyType, converterName.Span)
                };

                genericPropertyType = propertyWithValueConverterType.MakeGenericType(typeof(TBindingContext),
                    typeof(TValueType), propertyInfo.PropertyType);
            }

            return AddInstanceToCache<TProperty>(propertyName, Activator.CreateInstance(genericPropertyType, args));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IValueConverter GetValueConverter<TValueType>(Type sourceType, ReadOnlySpan<char> converterName)
        {
            var valueConverter = converterName.IsEmpty
                ? GetConverter(sourceType, typeof(TValueType))
                : GetConverter(sourceType, typeof(TValueType), converterName);

            if (valueConverter != null)
            {
                return valueConverter;
            }

            if (converterName.IsEmpty)
            {
                throw new NullReferenceException($"Converter is missing: From {sourceType} To {typeof(TValueType)}");
            }

            throw new NullReferenceException($"Converter '{converterName.ToString()}' not found.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IValueConverter GetConverter(Type sourceType, Type targetType)
        {
            return _valueConverters.FirstOrDefault(converter =>
                converter.SourceType == sourceType && converter.TargetType == targetType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IValueConverter GetConverter(Type sourceType, Type targetType, ReadOnlySpan<char> converterName)
        {
            foreach (var converter in _valueConverters)
            {
                if (converter.SourceType == sourceType &&
                    converter.TargetType == targetType &&
                    converterName.SequenceEqual(converter.Name))
                {
                    return converter;
                }
            }

            return null;
        }
    }
}