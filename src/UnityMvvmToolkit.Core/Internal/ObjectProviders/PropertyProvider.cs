using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.PropertyWrappers;
using UnityMvvmToolkit.Core.Internal.Helpers;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.ObjectProviders
{
    internal class PropertyProvider
    {
        /// <summary>
        /// Stores a FieldInfo.
        /// Key is a combination of the IBindingContext and the Property name hash codes.
        /// </summary>
        private readonly Dictionary<int, FieldInfo> _fields;

        /// <summary>
        /// Stores an array of FieldInfo for each IBindingContext.
        /// Key is the IBindingContext type.
        /// </summary>
        private readonly Dictionary<Type, FieldInfo[]> _bindingContextFields;

        /// <summary>
        /// Stores a queue of PropertyWithConverter instances.
        /// Key is the PropertyWithConverter hash code.
        /// </summary>
        private readonly Dictionary<int, Queue<IPropertyWrapper>> _propertiesWithConverter;

        private readonly HashSet<IPropertyValueConverter> _propertyValueConverters;

        internal PropertyProvider()
        {
            _fields = new Dictionary<int, FieldInfo>();
            _bindingContextFields = new Dictionary<Type, FieldInfo[]>();
            _propertiesWithConverter = new Dictionary<int, Queue<IPropertyWrapper>>();

            _propertyValueConverters = new HashSet<IPropertyValueConverter>();
        }

        public void RegisterValueConverter(IPropertyValueConverter converter)
        {
            _propertyValueConverters.Add(converter);
        }

        public void WarmupBindingContext(Type bindingContextType, PropertyInfo[] properties)
        {
            var fields = bindingContextType
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            var fieldsSpan = fields.AsSpan();
            var propertiesSpan = properties.AsSpan();

            for (var i = 0; i < propertiesSpan.Length; i++)
            {
                var propertyName = propertiesSpan[i].Name;

                for (var j = 0; j < fieldsSpan.Length; j++)
                {
                    var fieldInfo = fieldsSpan[j];

                    if (IsObservableBackingField(fieldInfo, propertyName))
                    {
                        var bindingContextTypeHash = bindingContextType.GetHashCode();
                        var propertyNameHash = propertyName.GetHashCode();

                        _fields.Add(HashCodeHelper.CombineHashCode(bindingContextTypeHash, propertyNameHash),
                            fieldInfo);
                    }
                }
            }

            _bindingContextFields.Add(bindingContextType, fields);
        }

        public IProperty<TValueType> GetProperty<TValueType>(PropertyBindingData bindingData, IBindingContext context)
        {
            var field = GetField<TValueType>(bindingData, context);
            if (field == null)
            {
                throw new NullReferenceException(
                    $"IProperty field for the '{bindingData.PropertyName}' property not found.");
            }

            return (IProperty<TValueType>) field;
        }

        public IReadOnlyProperty<TValueType> GetReadOnlyProperty<TValueType>(PropertyBindingData bindingData,
            IBindingContext context)
        {
            var field = GetField<TValueType>(bindingData, context);
            if (field == null)
            {
                throw new NullReferenceException(
                    $"IReadOnlyProperty field for the '{bindingData.PropertyName}' property not found.");
            }

            return (IReadOnlyProperty<TValueType>) field;
        }

        public void ReturnProperty(IPropertyWrapper propertyWrapper)
        {
            propertyWrapper.Reset();
            _propertiesWithConverter[propertyWrapper.GetHashCode()].Enqueue(propertyWrapper);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object GetField<TValueType>(PropertyBindingData bindingData, IBindingContext context)
        {
            var fieldInfo = GetFieldInfo(context, bindingData.PropertyName);

            var fieldValueType = fieldInfo.FieldType.GenericTypeArguments[0];
            if (fieldValueType == typeof(TValueType))
            {
                return fieldInfo.GetValue(context);
            }

            var propertyWrapperHashCode = IPropertyWrapper.GenerateHashCode(typeof(TValueType), fieldValueType);

            if (_propertiesWithConverter.TryGetValue(propertyWrapperHashCode, out var propertyWrappers))
            {
                if (propertyWrappers.Count > 0)
                {
                    return propertyWrappers
                        .Dequeue()
                        .SetProperty(fieldInfo.GetValue(context));
                }
            }
            else
            {
                _propertiesWithConverter.Add(propertyWrapperHashCode, new Queue<IPropertyWrapper>());
            }

            var args = new object[]
            {
                GetValueConverter<TValueType>(fieldValueType, bindingData.ConverterName)
            };

            var genericPropertyType =
                typeof(PropertyWithConverter<,>).MakeGenericType(typeof(TValueType), fieldValueType);

            return ((IPropertyWrapper) Activator.CreateInstance(genericPropertyType, args))
                .SetProperty(fieldInfo.GetValue(context));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private FieldInfo GetFieldInfo(IBindingContext context, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new NullReferenceException(propertyName);
            }

            var bindingContextType = context.GetType();

            var bindingContextTypeHash = bindingContextType.GetHashCode();
            var propertyNameHash = propertyName.GetHashCode();

            var fieldKey = HashCodeHelper.CombineHashCode(bindingContextTypeHash, propertyNameHash);

            if (_fields.TryGetValue(fieldKey, out var fieldInfo))
            {
                return fieldInfo;
            }

            fieldInfo = GetFieldInfo(bindingContextType, propertyName);
            _fields.Add(fieldKey, fieldInfo);

            return fieldInfo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private FieldInfo GetFieldInfo(Type bindingContextType, string propertyName)
        {
            if (_bindingContextFields.TryGetValue(bindingContextType, out var bindingContextFields) == false)
            {
                bindingContextFields = bindingContextType
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

                _bindingContextFields.Add(bindingContextType, bindingContextFields);
            }

            var fieldsSpan = bindingContextFields.AsSpan();

            for (var i = 0; i < fieldsSpan.Length; i++)
            {
                var fieldInfo = fieldsSpan[i];

                if (IsObservableBackingField(fieldInfo, propertyName))
                {
                    return fieldInfo;
                }
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsObservableBackingField(FieldInfo fieldInfo, string propertyName)
        {
            return IsObservableField(fieldInfo) && HasProperty(propertyName, fieldInfo.Name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsObservableField(FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType.IsGenericType == false)
            {
                return false;
            }

            var fieldType = fieldInfo.FieldType.GetGenericTypeDefinition();

            return fieldType == typeof(IProperty<>) || fieldType == typeof(IReadOnlyProperty<>) ||
                   fieldType == typeof(ObservableProperty<>);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasProperty(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> fieldName)
        {
            if (fieldName.Length > 1)
            {
                if (fieldName[0] == '_')
                {
                    return SequenceEqual(propertyName, fieldName[1..]);
                }

                if (fieldName[0] == 'm' && fieldName[1] == '_')
                {
                    return SequenceEqual(propertyName, fieldName[2..]);
                }
            }

            return SequenceEqual(propertyName, in fieldName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SequenceEqual(ReadOnlySpan<char> span1, in ReadOnlySpan<char> span2)
        {
            return span1.Equals(span2, StringComparison.OrdinalIgnoreCase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IPropertyValueConverter GetValueConverter<TValueType>(Type sourceType, string converterName)
        {
            var valueConverter = string.IsNullOrEmpty(converterName)
                ? GetConverter(sourceType, typeof(TValueType))
                : GetConverter(sourceType, typeof(TValueType), converterName);

            if (valueConverter != null)
            {
                return valueConverter;
            }

            if (string.IsNullOrEmpty(converterName))
            {
                throw new NullReferenceException($"Converter is missing: From {sourceType} To {typeof(TValueType)}");
            }

            throw new NullReferenceException($"Converter '{converterName}' not found.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IPropertyValueConverter GetConverter(Type sourceType, Type targetType)
        {
            foreach (var converter in _propertyValueConverters)
            {
                if (converter.SourceType == sourceType && converter.TargetType == targetType)
                {
                    return converter;
                }
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IPropertyValueConverter GetConverter(Type sourceType, Type targetType, string converterName)
        {
            foreach (var converter in _propertyValueConverters)
            {
                if (converter.SourceType == sourceType &&
                    converter.TargetType == targetType &&
                    converter.Name == converterName)
                {
                    return converter;
                }
            }

            return null;
        }
    }
}