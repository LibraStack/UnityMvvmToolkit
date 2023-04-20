using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Converters.ParameterValueConverters;
using UnityMvvmToolkit.Core.Enums;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Extensions;
using UnityMvvmToolkit.Core.Internal.Helpers;
using UnityMvvmToolkit.Core.Internal.Interfaces;
using UnityMvvmToolkit.Core.Internal.ObjectWrappers;

namespace UnityMvvmToolkit.Core.Internal.ObjectProviders
{
    internal class PropertyProvider : IDisposable
    {
        private readonly BindingContextMembersProvider _membersProvider;

        private readonly Dictionary<int, MemberInfo> _memberInfos;

        private readonly HashSet<int> _initializedBindingContexts;

        private readonly Dictionary<int, IValueConverter> _valueConvertersByHash;

        private readonly Dictionary<int, ICommandWrapper> _commandWrappers;
        private readonly Dictionary<int, Queue<IObjectWrapper>> _wrapperByConverter;

        private bool _isDisposed;

        internal PropertyProvider(BindingContextMembersProvider membersProvider, IValueConverter[] valueConverters)
        {
            _membersProvider = membersProvider;
            _memberInfos = new Dictionary<int, MemberInfo>();

            _initializedBindingContexts = new HashSet<int>();

            _valueConvertersByHash = new Dictionary<int, IValueConverter>();

            _commandWrappers = new Dictionary<int, ICommandWrapper>();

            _wrapperByConverter = new Dictionary<int, Queue<IObjectWrapper>>();

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
                    WarmupParameterValueConverter(converter, capacity, warmupType);
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
            if (_wrapperByConverter.ContainsKey(converterId))
            {
                throw new InvalidOperationException("Warm up only during the initialization phase.");
            }

            var itemsQueue = new Queue<IObjectWrapper>();

            var args = new object[] { converter };
            var genericPropertyType =
                typeof(PropertyWrapper<,>).MakeGenericType(converter.TargetType, converter.SourceType);

            for (var i = 0; i < capacity; i++)
            {
                itemsQueue
                    .Enqueue(CreatePropertyWrapperInstance(genericPropertyType, args, converterId, null));
            }

            _wrapperByConverter.Add(converterId, itemsQueue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WarmupParameterValueConverter(IParameterValueConverter converter, int capacity,
            WarmupType warmupType)
        {
            int converterId;

            switch (warmupType)
            {
                case WarmupType.OnlyByType:
                    converterId = HashCodeHelper.GetCommandWrapperConverterId(converter);
                    WarmupParameterValueConverter(converterId, converter, capacity);
                    break;
                case WarmupType.OnlyByName:
                    converterId = HashCodeHelper.GetCommandWrapperConverterId(converter, converter.Name);
                    WarmupParameterValueConverter(converterId, converter, capacity);
                    break;
                case WarmupType.ByTypeAndName:
                    converterId = HashCodeHelper.GetCommandWrapperConverterId(converter);
                    WarmupParameterValueConverter(converterId, converter, capacity);

                    converterId = HashCodeHelper.GetCommandWrapperConverterId(converter, converter.Name);
                    WarmupParameterValueConverter(converterId, converter, capacity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(warmupType), warmupType, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WarmupParameterValueConverter(int converterId, IParameterValueConverter converter, int capacity)
        {
            if (_wrapperByConverter.ContainsKey(converterId))
            {
                throw new InvalidOperationException("Warm up only during the initialization phase.");
            }

            var itemsQueue = new Queue<IObjectWrapper>();

            var args = new object[] { converter };
            var genericPropertyType = typeof(CommandWrapper<>).MakeGenericType(converter.TargetType);

            for (var i = 0; i < capacity; i++)
            {
                itemsQueue
                    .Enqueue(CreateCommandWrapperInstance(genericPropertyType, args, converterId, -1, null));
            }

            _wrapperByConverter.Add(converterId, itemsQueue);
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
            AssureIsNotDisposed();

            try
            {
                _wrapperByConverter[propertyWrapper.ConverterId].Enqueue(propertyWrapper);
            }
            finally
            {
                propertyWrapper.Reset();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCommand GetCommand<TCommand>(IBindingContext context, string propertyName) where TCommand : IBaseCommand
        {
            if (TryGetContextMemberInfo(context.GetType(), propertyName, out var memberInfo) == false ||
                memberInfo.MemberType != MemberTypes.Property)
            {
                throw new InvalidOperationException($"Command '{propertyName}' not found.");
            }

            var propertyInfo = (PropertyInfo) memberInfo;

            if (propertyInfo.PropertyType != typeof(TCommand))
            {
                throw new InvalidCastException(
                    $"Can not cast the {propertyInfo.PropertyType} command to the {typeof(TCommand)} command.");
            }

            return (TCommand) propertyInfo.GetValue(context);
        }

        public ICommandWrapper GetCommandWrapper(IBindingContext context, CommandBindingData bindingData)
        {
            var contextType = context.GetType();

            if (TryGetContextMemberInfo(contextType, bindingData.PropertyName, out var memberInfo) == false ||
                memberInfo.MemberType != MemberTypes.Property)
            {
                throw new InvalidOperationException($"Command '{bindingData.PropertyName}' not found.");
            }

            var propertyInfo = (PropertyInfo) memberInfo;
            var propertyType = propertyInfo.PropertyType;

            if (propertyType.IsGenericType == false ||
                propertyType.GetInterface(nameof(IBaseCommand)) == null)
            {
                throw new InvalidCastException(
                    $"Can not cast the {propertyType} command to the {typeof(ICommand<>)} command.");
            }

            var commandValueType = propertyType.GenericTypeArguments[0];

            var commandId =
                HashCodeHelper.GetCommandWrapperId(contextType, commandValueType, bindingData.PropertyName);

            if (_commandWrappers.TryGetValue(commandId, out var commandWrapper))
            {
                return commandWrapper
                    .RegisterParameter(bindingData.ElementId, bindingData.ParameterValue);
            }

            var converterId =
                HashCodeHelper.GetCommandWrapperConverterId(commandValueType, bindingData.ParameterConverterName);

            if (_wrapperByConverter.TryGetValue(converterId, out var commandWrappers))
            {
                if (commandWrappers.Count > 0)
                {
                    return commandWrappers
                        .Dequeue()
                        .AsCommandWrapper()
                        .SetCommand(commandId, propertyInfo.GetValue(context))
                        .RegisterParameter(bindingData.ElementId, bindingData.ParameterValue);
                }
            }
            else
            {
                _wrapperByConverter.Add(converterId, new Queue<IObjectWrapper>());
            }

            var args = new object[] { _valueConvertersByHash[converterId] };
            var commandWrapperType = typeof(CommandWrapper<>).MakeGenericType(commandValueType);
            var command = propertyInfo.GetValue(context);

            commandWrapper = CreateCommandWrapperInstance(commandWrapperType, args, converterId, commandId, command)
                .RegisterParameter(bindingData.ElementId, bindingData.ParameterValue);

            _commandWrappers.Add(commandId, commandWrapper);

            return commandWrapper;
        }

        public void ReturnCommandWrapper(ICommandWrapper commandWrapper, int elementId)
        {
            AssureIsNotDisposed();

            if (commandWrapper.UnregisterParameter(elementId) != 0)
            {
                return;
            }

            try
            {
                _commandWrappers.Remove(commandWrapper.CommandId);
                _wrapperByConverter[commandWrapper.ConverterId].Enqueue(commandWrapper);
            }
            finally
            {
                commandWrapper.Reset();
            }
        }

        public void Dispose()
        {
            _memberInfos.Clear();
            _initializedBindingContexts.Clear();

            _valueConvertersByHash.Clear();

            _wrapperByConverter.Clear();

            _isDisposed = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TProperty GetProperty<TProperty, TValueType>(IBindingContext context, PropertyBindingData bindingData)
        {
            if (TryGetContextMemberInfo(context.GetType(), bindingData.PropertyName, out var memberInfo) == false)
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

            if (_wrapperByConverter.TryGetValue(converterId, out var propertyWrappers))
            {
                if (propertyWrappers.Count > 0)
                {
                    return (TProperty) propertyWrappers
                        .Dequeue()
                        .AsPropertyWrapper()
                        .SetProperty(contextProperty);
                }
            }
            else
            {
                _wrapperByConverter.Add(converterId, new Queue<IObjectWrapper>());
            }

            var args = new object[] { _valueConvertersByHash[converterId] };
            var propertyWrapperType = typeof(PropertyWrapper<,>).MakeGenericType(targetType, sourceType);

            return (TProperty) CreatePropertyWrapperInstance(propertyWrapperType, args, converterId, contextProperty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetContextMemberInfo(Type contextType, string memberName, out MemberInfo memberInfo)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                throw new NullReferenceException(nameof(memberInfo)); // TODO: Move to up.
            }

            if (IsBindingContextInitialized(contextType) == false)
            {
                WarmupBindingContext(contextType);
            }

            return _memberInfos.TryGetValue(HashCodeHelper.GetMemberHashCode(contextType, memberName), out memberInfo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IPropertyWrapper CreatePropertyWrapperInstance(Type propertyWrapperType, object[] args,
            int converterId, object property)
        {
            var propertyWrapper = (IPropertyWrapper) Activator.CreateInstance(propertyWrapperType, args);

            return property is null
                ? propertyWrapper.SetConverterId(converterId)
                : propertyWrapper.SetConverterId(converterId).SetProperty(property);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ICommandWrapper CreateCommandWrapperInstance(Type commandWrapperType, object[] args,
            int converterId, int commandId, object command)
        {
            var commandWrapper = (ICommandWrapper) Activator.CreateInstance(commandWrapperType, args);

            return command is null
                ? commandWrapper.SetConverterId(converterId)
                : commandWrapper.SetConverterId(converterId).SetCommand(commandId, command);
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
                RegisterValueConverter(convertersSpan[i]);
            }

            if (_valueConvertersByHash.ContainsKey(typeof(ParameterToStrConverter).GetHashCode()) == false)
            {
                RegisterValueConverter(new ParameterToStrConverter());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RegisterValueConverter(IValueConverter valueConverter)
        {
            int converterHashByType;
            int converterHashByName;
            var converterTypeHash = valueConverter.GetType().GetHashCode();

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

            _valueConvertersByHash.Add(converterTypeHash, valueConverter);
            _valueConvertersByHash.Add(converterHashByType, valueConverter);
            _valueConvertersByHash.Add(converterHashByName, valueConverter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IValueConverter GetValueConverterByType(Type converterType)
        {
            return _valueConvertersByHash[converterType.GetHashCode()];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(PropertyProvider));
            }
        }
    }
}