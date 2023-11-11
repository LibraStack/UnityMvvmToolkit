using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;
using UnityMvvmToolkit.Core.Enums;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Extensions;
using UnityMvvmToolkit.Core.Internal.Helpers;
using UnityMvvmToolkit.Core.Internal.Interfaces;
using UnityMvvmToolkit.Core.Internal.ObjectWrappers;

namespace UnityMvvmToolkit.Core.Internal.ObjectHandlers
{
    internal sealed class ObjectWrapperHandler : IDisposable
    {
        private static readonly int ReadOnlyPropertyHashCode = typeof(IReadOnlyProperty<>).GetHashCode();

        private readonly ValueConverterHandler _valueConverterHandler;

        private readonly Dictionary<int, ICommandWrapper> _commandWrappers;
        private readonly Dictionary<int, Queue<IObjectWrapper>> _wrappersByConverter;

        private bool _isDisposed;

        public ObjectWrapperHandler(ValueConverterHandler valueConverterHandler)
        {
            _valueConverterHandler = valueConverterHandler;

            _commandWrappers = new Dictionary<int, ICommandWrapper>();
            _wrappersByConverter = new Dictionary<int, Queue<IObjectWrapper>>();
        }

        public void CreateValueConverterInstances<T>(int capacity, WarmupType warmupType) where T : IValueConverter
        {
            if (_valueConverterHandler.TryGetValueConverterByType(typeof(T), out var valueConverter) == false)
            {
                throw new NullReferenceException($"Converter '{typeof(T)}' not found");
            }

            switch (valueConverter)
            {
                case IPropertyValueConverter converter:
                    CreatePropertyValueConverterInstances(converter, capacity, warmupType);
                    break;

                case IParameterValueConverter converter:
                    CreateParameterValueConverterInstances(converter, capacity, warmupType);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public TProperty GetPropertyAs<TProperty, TValueType>(IBindingContext context, MemberInfo memberInfo)
            where TProperty : IBaseProperty
        {
            var property = memberInfo.GetMemberValue<IBaseProperty>(context, out var propertyType);

            var targetType = typeof(TValueType);
            var sourceType = propertyType.GenericTypeArguments[0];

            if (targetType == sourceType)
            {
                return (TProperty) property;
            }

            if (targetType.IsValueType || sourceType.IsValueType)
            {
                throw new InvalidOperationException(
                    $"{nameof(GetPropertyAs)} is not supported for value types. Use {typeof(PropertyValueConverter<,>).Name} instead.");
            }

            if (targetType.IsAssignableFrom(sourceType) == false)
            {
                throw new InvalidCastException($"Can not cast the '{sourceType}' to the '{targetType}'.");
            }

            var converterId = HashCodeHelper.GetPropertyWrapperConverterId(targetType, sourceType);

            var isProperty = property is IProperty;

            var wrapperId = isProperty ? converterId : GetReadOnlyWrapperId(converterId);

            if (TryGetObjectWrapper(wrapperId, out var objectWrapper))
            {
                return (TProperty) objectWrapper
                    .AsPropertyWrapper()
                    .SetProperty(property);
            }

            var wrapperType = isProperty
                ? typeof(PropertyCastWrapper<,>).MakeGenericType(sourceType, targetType)
                : typeof(ReadOnlyPropertyCastWrapper<,>).MakeGenericType(sourceType, targetType);

            return (TProperty) ObjectWrapperHelper.CreatePropertyWrapper(wrapperType, default, converterId, property);
        }

        public TProperty GetProperty<TProperty, TValueType>(IBindingContext context, BindingData bindingData,
            MemberInfo memberInfo) where TProperty : IBaseProperty
        {
            var property = memberInfo.GetMemberValue<IBaseProperty>(context, out var propertyType);

            var targetType = typeof(TValueType);
            var sourceType = propertyType.GenericTypeArguments[0];

            if (targetType == sourceType && string.IsNullOrWhiteSpace(bindingData.ConverterName))
            {
                return (TProperty) property;
            }

            var converterId =
                HashCodeHelper.GetPropertyWrapperConverterId(targetType, sourceType, bindingData.ConverterName);

            var isProperty = property is IProperty;

            var wrapperId = isProperty ? converterId : GetReadOnlyWrapperId(converterId);

            if (TryGetObjectWrapper(wrapperId, out var objectWrapper))
            {
                return (TProperty) objectWrapper
                    .AsPropertyWrapper()
                    .SetProperty(property);
            }

            if (_valueConverterHandler.TryGetValueConverterById(converterId, out var valueConverter) == false)
            {
                throw new NullReferenceException(
                    $"Property value converter from '{sourceType}' to '{targetType}' not found.");
            }

            var args = new object[] { valueConverter };

            var wrapperType = isProperty
                ? typeof(PropertyConvertWrapper<,>).MakeGenericType(sourceType, targetType)
                : typeof(ReadOnlyPropertyConvertWrapper<,>).MakeGenericType(sourceType, targetType);

            return (TProperty) ObjectWrapperHelper.CreatePropertyWrapper(wrapperType, args, converterId, property);
        }

        public TCommand GetCommand<TCommand>(IBindingContext context, MemberInfo memberInfo)
            where TCommand : IBaseCommand
        {
            return memberInfo.GetMemberValue<TCommand>(context, out _);
        }

        public ICommandWrapper GetCommandWrapper(IBindingContext context, CommandBindingData bindingData,
            MemberInfo memberInfo)
        {
            var command = memberInfo.GetMemberValue<IBaseCommand>(context, out var commandType);

            if (commandType.IsGenericType == false ||
                commandType.GetInterface(nameof(IBaseCommand)) == null)
            {
                throw new InvalidCastException(
                    $"Can not cast the '{commandType}' command to the '{typeof(ICommand<>)}' command.");
            }

            var commandValueType = commandType.GenericTypeArguments[0];

            var commandId =
                HashCodeHelper.GetCommandWrapperId(context.GetType(), commandValueType, bindingData.PropertyName);

            if (_commandWrappers.TryGetValue(commandId, out var commandWrapper))
            {
                return commandWrapper
                    .RegisterParameter(bindingData.ElementId, bindingData.ParameterValue);
            }

            var converterId =
                HashCodeHelper.GetCommandWrapperConverterId(commandValueType, bindingData.ConverterName);

            if (TryGetObjectWrapper(converterId, out var objectWrapper))
            {
                return objectWrapper
                    .AsCommandWrapper()
                    .SetCommand(commandId, command)
                    .RegisterParameter(bindingData.ElementId, bindingData.ParameterValue);
            }

            if (_valueConverterHandler.TryGetValueConverterById(converterId, out var valueConverter) == false)
            {
                throw new NullReferenceException(
                    $"Parameter value converter to '{commandValueType}' not found.");
            }

            var args = new object[] { valueConverter };
            var wrapperType = typeof(CommandWrapper<>).MakeGenericType(commandValueType);

            commandWrapper = ObjectWrapperHelper
                .CreateCommandWrapper(wrapperType, args, converterId, commandId, command)
                .RegisterParameter(bindingData.ElementId, bindingData.ParameterValue);

            _commandWrappers.Add(commandId, commandWrapper);

            return commandWrapper;
        }

        public void ReturnProperty(IPropertyWrapper propertyWrapper)
        {
            AssureIsNotDisposed();

            ReturnObjectWrapper(propertyWrapper);
        }

        public void ReturnCommandWrapper(ICommandWrapper commandWrapper, int elementId)
        {
            AssureIsNotDisposed();

            if (commandWrapper.UnregisterParameter(elementId) != 0)
            {
                return;
            }

            _commandWrappers.Remove(commandWrapper.CommandId);

            ReturnObjectWrapper(commandWrapper);
        }

        public void Dispose()
        {
            _isDisposed = true;

            _wrappersByConverter.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreatePropertyValueConverterInstances(IPropertyValueConverter converter, int capacity,
            WarmupType warmupType) // TODO: Refactor.
        {
            int converterId;

            switch (warmupType)
            {
                case WarmupType.OnlyByType:
                    converterId = HashCodeHelper.GetPropertyWrapperConverterId(converter);
                    CreatePropertyValueConverterInstances(converterId, converter, capacity);
                    break;

                case WarmupType.OnlyByName:
                    converterId = HashCodeHelper.GetPropertyWrapperConverterId(converter, converter.Name);
                    CreatePropertyValueConverterInstances(converterId, converter, capacity);
                    break;

                case WarmupType.ByTypeAndName:
                    converterId = HashCodeHelper.GetPropertyWrapperConverterId(converter);
                    CreatePropertyValueConverterInstances(converterId, converter, capacity);

                    converterId = HashCodeHelper.GetPropertyWrapperConverterId(converter, converter.Name);
                    CreatePropertyValueConverterInstances(converterId, converter, capacity);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(warmupType), warmupType, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreatePropertyValueConverterInstances(int converterId, IPropertyValueConverter converter,
            int capacity)
        {
            if (_wrappersByConverter.ContainsKey(converterId))
            {
                throw new InvalidOperationException("Warm up only during the initialization phase.");
            }

            var itemsQueue = new Queue<IObjectWrapper>();

            var args = new object[] { converter };
            var wrapperType = typeof(PropertyConvertWrapper<,>).MakeGenericType(converter.SourceType, converter.TargetType);

            for (var i = 0; i < capacity; i++)
            {
                itemsQueue
                    .Enqueue(ObjectWrapperHelper.CreatePropertyWrapper(wrapperType, args, converterId, null));
            }

            _wrappersByConverter.Add(converterId, itemsQueue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateParameterValueConverterInstances(IParameterValueConverter converter, int capacity,
            WarmupType warmupType) // TODO: Refactor.
        {
            int converterId;

            switch (warmupType)
            {
                case WarmupType.OnlyByType:
                    converterId = HashCodeHelper.GetCommandWrapperConverterId(converter);
                    CreateParameterValueConverterInstances(converterId, converter, capacity);
                    break;

                case WarmupType.OnlyByName:
                    converterId = HashCodeHelper.GetCommandWrapperConverterId(converter, converter.Name);
                    CreateParameterValueConverterInstances(converterId, converter, capacity);
                    break;

                case WarmupType.ByTypeAndName:
                    converterId = HashCodeHelper.GetCommandWrapperConverterId(converter);
                    CreateParameterValueConverterInstances(converterId, converter, capacity);

                    converterId = HashCodeHelper.GetCommandWrapperConverterId(converter, converter.Name);
                    CreateParameterValueConverterInstances(converterId, converter, capacity);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(warmupType), warmupType, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateParameterValueConverterInstances(int converterId, IParameterValueConverter converter,
            int capacity)
        {
            if (_wrappersByConverter.ContainsKey(converterId))
            {
                throw new InvalidOperationException("Warm up only during the initialization phase.");
            }

            var itemsQueue = new Queue<IObjectWrapper>();

            var args = new object[] { converter };
            var wrapperType = typeof(CommandWrapper<>).MakeGenericType(converter.TargetType);

            for (var i = 0; i < capacity; i++)
            {
                itemsQueue
                    .Enqueue(ObjectWrapperHelper.CreateCommandWrapper(wrapperType, args, converterId, -1, null));
            }

            _wrappersByConverter.Add(converterId, itemsQueue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetObjectWrapper(int wrapperId, out IObjectWrapper objectWrapper)
        {
            if (_wrappersByConverter.TryGetValue(wrapperId, out var objectWrappers))
            {
                if (objectWrappers.Count > 0)
                {
                    objectWrapper = objectWrappers.Dequeue();
                    return true;
                }
            }
            else
            {
                _wrappersByConverter.Add(wrapperId, new Queue<IObjectWrapper>());
            }

            objectWrapper = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReturnObjectWrapper(IObjectWrapper wrapper)
        {
            wrapper.Reset();

            switch (wrapper)
            {
                case IProperty:
                case ICommandWrapper:
                    _wrappersByConverter[wrapper.ConverterId].Enqueue(wrapper);
                    break;

                default:
                    _wrappersByConverter[GetReadOnlyWrapperId(wrapper.ConverterId)].Enqueue(wrapper);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(ObjectWrapperHandler));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetReadOnlyWrapperId(int wrapperConverterId)
        {
            return HashCodeHelper.CombineHashCode(wrapperConverterId, ReadOnlyPropertyHashCode);
        }
    }
}