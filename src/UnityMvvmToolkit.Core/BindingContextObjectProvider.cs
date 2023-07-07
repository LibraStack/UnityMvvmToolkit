using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Enums;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal;
using UnityMvvmToolkit.Core.Internal.Extensions;
using UnityMvvmToolkit.Core.Internal.Interfaces;
using UnityMvvmToolkit.Core.Internal.ObjectHandlers;

namespace UnityMvvmToolkit.Core
{
    public sealed class BindingContextObjectProvider : IObjectProvider, IDisposable
    {
        private readonly ObjectWrapperHandler _objectWrapperHandler;
        private readonly ValueConverterHandler _valueConverterHandler;
        private readonly BindingContextHandler _bindingContextHandler;

        private readonly IReadOnlyDictionary<Type, object> _collectionItemTemplates;

        public BindingContextObjectProvider(IValueConverter[] converters,
            IReadOnlyDictionary<Type, object> collectionItemTemplates = null)
        {
            _valueConverterHandler = new ValueConverterHandler(converters);
            _objectWrapperHandler = new ObjectWrapperHandler(_valueConverterHandler);
            _bindingContextHandler = new BindingContextHandler(new BindingContextMemberProvider());

            _collectionItemTemplates = collectionItemTemplates ?? ImmutableDictionary.Empty<Type, object>();
        }

        public IObjectProvider WarmupAssemblyViewModels()
        {
            return WarmupAssemblyViewModels(Assembly.GetCallingAssembly());
        }

        public IObjectProvider WarmupAssemblyViewModels(Assembly assembly)
        {
            var assemblyTypes = assembly
                .GetTypes()
                .Where(type => type.IsInterface == false && type.IsAbstract == false &&
                               typeof(IBindingContext).IsAssignableFrom(type));

            foreach (var type in assemblyTypes)
            {
                WarmupViewModel(type);
            }

            return this;
        }

        public IObjectProvider WarmupViewModel<TBindingContext>() where TBindingContext : IBindingContext
        {
            return WarmupViewModel(typeof(TBindingContext));
        }

        public IObjectProvider WarmupViewModel(Type bindingContextType)
        {
            if (bindingContextType.IsInterface || bindingContextType.IsAbstract ||
                typeof(IBindingContext).IsAssignableFrom(bindingContextType) == false)
            {
                throw new InvalidOperationException($"Can not warmup {bindingContextType.Name}.");
            }

            if (_bindingContextHandler.TryRegisterBindingContext(bindingContextType) == false)
            {
                throw new InvalidOperationException($"{bindingContextType.Name} already warmed up.");
            }

            return this;
        }

        public IObjectProvider WarmupValueConverter<T>(int capacity, WarmupType warmupType = WarmupType.OnlyByType)
            where T : IValueConverter
        {
            _objectWrapperHandler.CreateValueConverterInstances<T>(capacity, warmupType);

            return this;
        }

        public IProperty<TValueType> RentProperty<TValueType>(IBindingContext context, PropertyBindingData bindingData)
        {
            EnsureBindingDataValid(bindingData);

            return GetProperty<IProperty<TValueType>, TValueType>(context, bindingData);
        }

        public void ReturnProperty<TValueType>(IProperty<TValueType> property)
        {
            ReturnBaseProperty(property);
        }

        public IReadOnlyProperty<TValueType> RentReadOnlyProperty<TValueType>(IBindingContext context,
            PropertyBindingData bindingData)
        {
            EnsureBindingDataValid(bindingData);

            return GetProperty<IReadOnlyProperty<TValueType>, TValueType>(context, bindingData);
        }

        public void ReturnReadOnlyProperty<TValueType>(IReadOnlyProperty<TValueType> property)
        {
            ReturnBaseProperty(property);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCommand GetCommand<TCommand>(IBindingContext context, string propertyName) where TCommand : IBaseCommand
        {
            EnsureIsNotNullOrWhiteSpace(propertyName, nameof(propertyName));

            if (TryGetContextMemberInfo(context.GetType(), propertyName, out var memberInfo) == false)
            {
                throw new InvalidOperationException($"Command '{propertyName}' not found.");
            }

            return _objectWrapperHandler.GetCommand<TCommand>(context, memberInfo);
        }

        public IBaseCommand RentCommandWrapper(IBindingContext context, CommandBindingData bindingData)
        {
            EnsureIsNotNullOrWhiteSpace(bindingData.ParameterValue,
                $"Command '{bindingData.PropertyName}' has no parameter. Use {nameof(GetCommand)} instead.");

            if (TryGetContextMemberInfo(context.GetType(), bindingData.PropertyName, out var memberInfo) == false)
            {
                throw new InvalidOperationException($"Command '{bindingData.PropertyName}' not found.");
            }

            return _objectWrapperHandler.GetCommandWrapper(context, bindingData, memberInfo);
        }

        public void ReturnCommandWrapper(IBaseCommand command, CommandBindingData bindingData)
        {
            if (command is ICommandWrapper commandWrapper)
            {
                _objectWrapperHandler.ReturnCommandWrapper(commandWrapper, bindingData.ElementId);
            }
        }

        public TValue GetCollectionItemTemplate<TKey, TValue>()
        {
            if (_collectionItemTemplates.TryGetValue(typeof(TKey), out var itemTemplate))
            {
                return (TValue) itemTemplate;
            }

            throw new NullReferenceException($"Item template for '{typeof(TKey)}' not found.");
        }

        public void Dispose()
        {
            _objectWrapperHandler.Dispose();
            _valueConverterHandler.Dispose();
            _bindingContextHandler.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TProperty GetProperty<TProperty, TValueType>(IBindingContext context, BindingData bindingData)
            where TProperty : IBaseProperty
        {
            if (TryGetContextMemberInfo(context.GetType(), bindingData.PropertyName, out var memberInfo) == false)
            {
                throw new InvalidOperationException($"Property '{bindingData.PropertyName}' not found.");
            }

            return _objectWrapperHandler.GetProperty<TProperty, TValueType>(context, bindingData, memberInfo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetContextMemberInfo(Type contextType, string memberName, out MemberInfo memberInfo)
        {
            return _bindingContextHandler.TryGetContextMemberInfo(contextType, memberName, out memberInfo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReturnBaseProperty(IBaseProperty property)
        {
            if (property is IPropertyWrapper propertyWrapper)
            {
                _objectWrapperHandler.ReturnProperty(propertyWrapper);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureBindingDataValid(BindingData bindingData)
        {
            EnsureIsNotNullOrWhiteSpace(bindingData.PropertyName, nameof(bindingData.PropertyName));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureIsNotNullOrWhiteSpace(string str, string message)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new NullReferenceException(message);
            }
        }
    }
}