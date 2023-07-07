using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        public TProperty GetProperty<TProperty, TValueType>(IBindingContext context, BindingData bindingData,
            MemberInfo memberInfo) where TProperty : IBaseProperty
        {
            var property = GetMemberValue<IBaseProperty>(context, memberInfo, out var propertyType);

            var targetType = typeof(TValueType);
            var sourceType = propertyType.GenericTypeArguments[0];

            if (targetType == sourceType && string.IsNullOrWhiteSpace(bindingData.ConverterName))
            {
                return (TProperty) property;
            }

            var converterId =
                HashCodeHelper.GetPropertyWrapperConverterId(targetType, sourceType, bindingData.ConverterName);

            if (_wrappersByConverter.TryGetValue(converterId, out var propertyWrappers))
            {
                if (propertyWrappers.Count > 0)
                {
                    return (TProperty) propertyWrappers
                        .Dequeue()
                        .AsPropertyWrapper()
                        .SetProperty(property);
                }
            }
            else
            {
                _wrappersByConverter.Add(converterId, new Queue<IObjectWrapper>());
            }

            if (_valueConverterHandler.TryGetValueConverterById(converterId, out var valueConverter) == false)
            {
                throw new NullReferenceException(
                    $"Property value converter from '{sourceType}' to '{targetType}' not found.");
            }

            var args = new object[] { valueConverter };

            var wrapperType = property is IProperty
                ? typeof(PropertyWrapper<,>).MakeGenericType(sourceType, targetType)
                : typeof(ReadOnlyPropertyWrapper<,>).MakeGenericType(sourceType, targetType);

            return (TProperty) ObjectWrapperHelper.CreatePropertyWrapper(wrapperType, args, converterId, property);
        }

        public TCommand GetCommand<TCommand>(IBindingContext context, MemberInfo memberInfo)
            where TCommand : IBaseCommand
        {
            return GetMemberValue<TCommand>(context, memberInfo, out _);
        }

        public ICommandWrapper GetCommandWrapper(IBindingContext context, CommandBindingData bindingData,
            MemberInfo memberInfo)
        {
            var command = GetMemberValue<IBaseCommand>(context, memberInfo, out var commandType);

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

            if (_wrappersByConverter.TryGetValue(converterId, out var commandWrappers))
            {
                if (commandWrappers.Count > 0)
                {
                    return commandWrappers
                        .Dequeue()
                        .AsCommandWrapper()
                        .SetCommand(commandId, command)
                        .RegisterParameter(bindingData.ElementId, bindingData.ParameterValue);
                }
            }
            else
            {
                _wrappersByConverter.Add(converterId, new Queue<IObjectWrapper>());
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

            ReturnWrapper(propertyWrapper);
        }

        public void ReturnCommandWrapper(ICommandWrapper commandWrapper, int elementId)
        {
            AssureIsNotDisposed();

            if (commandWrapper.UnregisterParameter(elementId) != 0)
            {
                return;
            }

            _commandWrappers.Remove(commandWrapper.CommandId);
            ReturnWrapper(commandWrapper);
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
            var wrapperType = typeof(PropertyWrapper<,>).MakeGenericType(converter.SourceType, converter.TargetType);

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
        private void ReturnWrapper(IObjectWrapper wrapper)
        {
            wrapper.Reset();
            _wrappersByConverter[wrapper.ConverterId].Enqueue(wrapper);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T GetMemberValue<T>(IBindingContext context, MemberInfo memberInfo, out Type memberType)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(ObjectWrapperHandler));
            }
        }
    }
}