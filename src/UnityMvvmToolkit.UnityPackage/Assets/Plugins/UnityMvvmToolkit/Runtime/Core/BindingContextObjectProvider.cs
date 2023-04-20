using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Enums;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Interfaces;
using UnityMvvmToolkit.Core.Internal.ObjectProviders;

namespace UnityMvvmToolkit.Core
{
    public sealed class BindingContextObjectProvider : IObjectProvider, IDisposable
    {
        private readonly PropertyProvider _propertyProvider;

        public BindingContextObjectProvider(IValueConverter[] converters)
        {
            _propertyProvider = new PropertyProvider(new BindingContextMembersProvider(), converters);
        }

        public IObjectProvider WarmupAssemblyViewModels()
        {
            var assemblyTypes = Assembly
                .GetExecutingAssembly()
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
            _propertyProvider.WarmupBindingContext(bindingContextType);

            return this;
        }

        public IObjectProvider WarmupValueConverter<T>(int capacity, WarmupType warmupType = WarmupType.OnlyByType)
            where T : IValueConverter
        {
            _propertyProvider.WarmupValueConverter<T>(capacity, warmupType);

            return this;
        }

        public IProperty<TValueType> RentProperty<TValueType>(IBindingContext context, PropertyBindingData bindingData)
        {
            return _propertyProvider.GetProperty<TValueType>(context, bindingData);
        }

        public void ReturnProperty<TValueType>(IProperty<TValueType> property)
        {
            ReturnBaseProperty(property);
        }

        public IReadOnlyProperty<TValueType> RentReadOnlyProperty<TValueType>(IBindingContext context,
            PropertyBindingData bindingData)
        {
            return _propertyProvider.GetReadOnlyProperty<TValueType>(context, bindingData);
        }

        public void ReturnReadOnlyProperty<TValueType>(IReadOnlyProperty<TValueType> property)
        {
            ReturnBaseProperty(property);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCommand GetCommand<TCommand>(IBindingContext context, string propertyName) where TCommand : IBaseCommand
        {
            return _propertyProvider.GetCommand<TCommand>(context, propertyName);
        }

        public IBaseCommand RentCommandWrapper(IBindingContext context, CommandBindingData bindingData)
        {
            if (string.IsNullOrEmpty(bindingData.ParameterValue))
            {
                throw new InvalidOperationException(
                    $"Command '{bindingData.PropertyName}' has no parameter. Use {nameof(GetCommand)} instead.");
            }

            return _propertyProvider.GetCommandWrapper(context, bindingData);
        }

        public void ReturnCommandWrapper(IBaseCommand command, CommandBindingData bindingData)
        {
            if (command is ICommandWrapper commandWrapper)
            {
                _propertyProvider.ReturnCommandWrapper(commandWrapper, bindingData.ElementId);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReturnBaseProperty(IBaseProperty property)
        {
            if (property is IPropertyWrapper propertyWrapper)
            {
                _propertyProvider.ReturnProperty(propertyWrapper);
            }
        }

        public void Dispose()
        {
            _propertyProvider?.Dispose();
        }
    }
}