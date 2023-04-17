using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Enums;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.PropertyWrappers;
using UnityMvvmToolkit.Core.Internal.Helpers;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.ObjectProviders
{
    internal class PropertyProvider
    {
        private readonly BindingContextMembersProvider _membersProvider;

        private readonly Dictionary<int, MemberInfo> _memberInfos;

        private readonly HashSet<int> _initializedBindingContexts;

        private readonly IValueConverter[] _valueConverters;
        private readonly Dictionary<int, IValueConverter> _valueConvertersByHash;
        
        private readonly Dictionary<int, Queue<ICommandWrapper>> _commandWrapperByConverter;
        private readonly Dictionary<int, Queue<IPropertyWrapper>> _propertyWrapperByConverter;

        internal PropertyProvider(BindingContextMembersProvider membersProvider, IValueConverter[] valueConverters)
        {
            _membersProvider = membersProvider;
            _memberInfos = new Dictionary<int, MemberInfo>();

            _initializedBindingContexts = new HashSet<int>();

            _valueConverters = valueConverters;
            _valueConvertersByHash = new Dictionary<int, IValueConverter>();

            _commandWrapperByConverter = new Dictionary<int, Queue<ICommandWrapper>>();
            _propertyWrapperByConverter = new Dictionary<int, Queue<IPropertyWrapper>>();

            RegisterValueConverters(valueConverters);
        }

        public void WarmupBindingContext(Type bindingContextType)
        {
            if (_initializedBindingContexts.Add(bindingContextType.GetHashCode()))
            {
                _membersProvider.GetBindingContextMembers(bindingContextType, _memberInfos);
            }
        }

        public void WarmupValueConverter<T>(int capacity, WarmupType warmupType) where T : IValueConverter
        {
            var valueConverter = GetValueConverterByType(typeof(T));

            if (valueConverter == default)
            {
                throw new NullReferenceException($"Converter '{typeof(T)}' not found.");
            }

            switch (valueConverter)
            {
                case IPropertyValueConverter converter:
                    WarmupPropertyValueConverter(converter, capacity, warmupType);
                    break;
                case IParameterValueConverter converter:
                    WarmupParameterValueConverter(converter, capacity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WarmupPropertyValueConverter(IPropertyValueConverter converter, int capacity,
            WarmupType warmupType)
        {
            int converterId;

            switch (warmupType)
            {
                case WarmupType.OnlyByType:
                    converterId = HashCodeHelper.GetPropertyWrapperConverterId(converter);
                    WarmupPropertyValueConverter(converterId, converter, capacity);
                    break;
                case WarmupType.OnlyByName:
                    converterId = HashCodeHelper.GetPropertyWrapperConverterId(converter, converter.Name);
                    WarmupPropertyValueConverter(converterId, converter, capacity);
                    break;
                case WarmupType.ByTypeAndName:
                    converterId = HashCodeHelper.GetPropertyWrapperConverterId(converter);
                    WarmupPropertyValueConverter(converterId, converter, capacity);

                    converterId = HashCodeHelper.GetPropertyWrapperConverterId(converter, converter.Name);
                    WarmupPropertyValueConverter(converterId, converter, capacity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(warmupType), warmupType, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WarmupPropertyValueConverter(int converterId, IPropertyValueConverter converter, int capacity)
        {
            if (_propertyWrapperByConverter.ContainsKey(converterId))
            {
                throw new InvalidOperationException(
                    "Warm up the value converters only during the initialization phase.");
            }

            var itemsQueue = new Queue<IPropertyWrapper>();

            var args = new object[] { converter };
            var genericPropertyType = typeof(PropertyWithConverter<,>)
                .MakeGenericType(converter.TargetType, converter.SourceType);

            for (var i = 0; i < capacity; i++)
            {
                itemsQueue
                    .Enqueue(CreatePropertyWrapperInstance(genericPropertyType, args, converterId, null));
            }

            _propertyWrapperByConverter.Add(converterId, itemsQueue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WarmupParameterValueConverter(IParameterValueConverter parameterConverter, int capacity)
        {
            throw new NotImplementedException();
        }

        public IProperty<TValueType> GetProperty<TValueType>(IBindingContext context, PropertyBindingData bindingData)
        {
            return GetProperty<IProperty<TValueType>, TValueType>(context, bindingData);
        }

        public IReadOnlyProperty<TValueType> GetReadOnlyProperty<TValueType>(IBindingContext context,
            PropertyBindingData bindingData)
        {
            return GetProperty<IReadOnlyProperty<TValueType>, TValueType>(context, bindingData);
        }

        public void ReturnProperty(IPropertyWrapper propertyWrapper)
        {
            propertyWrapper.Reset();
            _propertyWrapperByConverter[propertyWrapper.ConverterId].Enqueue(propertyWrapper);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCommand GetCommand<TCommand>(IBindingContext context, string propertyName) where TCommand : IBaseCommand
        {
            var contextType = context.GetType();

            if (IsBindingContextInitialized(contextType) == false)
            {
                WarmupBindingContext(contextType);
            }

            var memberHash = HashCodeHelper.GetMemberHashCode(contextType, propertyName);

            if (_memberInfos.TryGetValue(memberHash, out var memberInfo) == false ||
                memberInfo.MemberType != MemberTypes.Property)
            {
                throw new InvalidOperationException($"Property '{propertyName}' not found.");
            }

            var propertyInfo = (PropertyInfo) memberInfo;

            if (propertyInfo.PropertyType != typeof(TCommand))
            {
                throw new InvalidCastException(
                    $"Can not cast the {propertyInfo.PropertyType} command to the {typeof(TCommand)} command.");
            }

            return (TCommand) propertyInfo.GetValue(context);
        }

        public ICommandWrapperWithParameter GetCommandWrapper(IBindingContext context, CommandBindingData bindingData)
        {
            throw new NotImplementedException();
        }

        public void ReturnCommandWrapper(ICommandWrapper commandWrapper)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TProperty GetProperty<TProperty, TValueType>(IBindingContext context, PropertyBindingData bindingData)
        {
            var contextType = context.GetType();

            if (IsBindingContextInitialized(contextType) == false)
            {
                WarmupBindingContext(contextType);
            }

            var memberHash = HashCodeHelper.GetMemberHashCode(contextType, bindingData.PropertyName);

            if (_memberInfos.TryGetValue(memberHash, out var memberInfo) == false)
            {
                throw new InvalidOperationException($"Property '{bindingData.PropertyName}' not found.");
            }

            var targetType = typeof(TValueType);
            var contextProperty = GetMemberValue(context, memberInfo, out var sourceType);

            if (targetType == sourceType)
            {
                return (TProperty) contextProperty;
            }

            var converterId =
                HashCodeHelper.GetPropertyWrapperConverterId(targetType, sourceType, bindingData.ConverterName);

            if (_propertyWrapperByConverter.TryGetValue(converterId, out var propertyWrappers))
            {
                if (propertyWrappers.Count > 0)
                {
                    return (TProperty) propertyWrappers.Dequeue().SetProperty(contextProperty);
                }
            }
            else
            {
                _propertyWrapperByConverter.Add(converterId, new Queue<IPropertyWrapper>());
            }

            var args = new object[] { _valueConvertersByHash[converterId] };
            var genericPropertyType = typeof(PropertyWithConverter<,>).MakeGenericType(targetType, sourceType);

            return (TProperty) CreatePropertyWrapperInstance(genericPropertyType, args, converterId, contextProperty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IPropertyWrapper CreatePropertyWrapperInstance(Type genericPropertyType, object[] args,
            int converterId, object property)
        {
            var propertyWrapper = (IPropertyWrapper) Activator.CreateInstance(genericPropertyType, args);

            return property is null
                ? propertyWrapper.SetConverterId(converterId)
                : propertyWrapper.SetConverterId(converterId).SetProperty(property);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object GetMemberValue(IBindingContext context, MemberInfo memberInfo, out Type memberValueType)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                {
                    var fieldInfo = (FieldInfo) memberInfo;
                    memberValueType = fieldInfo.FieldType.GenericTypeArguments[0];

                    return fieldInfo.GetValue(context);
                }
                case MemberTypes.Property:
                {
                    var propertyInfo = (PropertyInfo) memberInfo;
                    memberValueType = propertyInfo.PropertyType.GenericTypeArguments[0];

                    return propertyInfo.GetValue(context);
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsBindingContextInitialized(Type bindingContextType)
        {
            return _initializedBindingContexts.Contains(bindingContextType.GetHashCode());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RegisterValueConverters(IValueConverter[] converters)
        {
            var convertersSpan = converters.AsSpan();

            for (var i = 0; i < convertersSpan.Length; i++)
            {
                var valueConverter = convertersSpan[i];

                int converterHashByType;
                int converterHashByName;

                switch (valueConverter)
                {
                    case IPropertyValueConverter converter:
                        converterHashByType = HashCodeHelper.GetPropertyConverterHashCode(converter);
                        converterHashByName = HashCodeHelper.GetPropertyConverterHashCode(converter, converter.Name);
                        break;
                    case IParameterValueConverter converter:
                        converterHashByType = HashCodeHelper.GetParameterConverterHashCode(converter);
                        converterHashByName = HashCodeHelper.GetParameterConverterHashCode(converter, converter.Name);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _valueConvertersByHash.Add(converterHashByType, valueConverter);
                _valueConvertersByHash.Add(converterHashByName, valueConverter);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IValueConverter GetValueConverterByType(Type converterType)
        {
            var convertersSpan = _valueConverters.AsSpan();

            for (var i = 0; i < convertersSpan.Length; i++)
            {
                var converter = convertersSpan[i];

                if (converter.GetType() == converterType)
                {
                    return converter;
                }
            }

            return default;
        }
    }
}