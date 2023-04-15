using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Interfaces;
using UnityMvvmToolkit.Core.Internal.ObjectProviders;

namespace UnityMvvmToolkit.Core
{
    public class BindingContextObjectProvider : IObjectProvider
    {
        private readonly CommandProvider _commandProvider;
        private readonly PropertyProvider _propertyProvider;

        public BindingContextObjectProvider(IValueConverter[] converters)
        {
            _commandProvider = new CommandProvider();
            _propertyProvider = new PropertyProvider();

            RegisterValueConverters(converters);
        }

        public void WarmupAssemblyViewModels()
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
        }

        public void WarmupViewModel<TBindingContext>() where TBindingContext : IBindingContext
        {
            WarmupViewModel(typeof(TBindingContext));
        }

        public void WarmupViewModel(Type bindingContextType)
        {
            var properties = bindingContextType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            _commandProvider.WarmupBindingContext(bindingContextType, properties);
            _propertyProvider.WarmupBindingContext(bindingContextType, properties);
        }

        public IProperty<TValueType> RentProperty<TValueType>(IBindingContext context, PropertyBindingData bindingData)
        {
            return _propertyProvider.GetProperty<TValueType>(bindingData, context);
        }

        public void ReturnProperty<TValueType>(IProperty<TValueType> property)
        {
            if (property is IPropertyWrapper propertyWrapper)
            {
                _propertyProvider.ReturnProperty(propertyWrapper);
            }
        }

        public IReadOnlyProperty<TValueType> RentReadOnlyProperty<TValueType>(IBindingContext context,
            PropertyBindingData bindingData)
        {
            return _propertyProvider.GetReadOnlyProperty<TValueType>(bindingData, context);
        }

        public void ReturnReadOnlyProperty<TValueType>(IReadOnlyProperty<TValueType> property)
        {
            if (property is IPropertyWrapper propertyWrapper)
            {
                _propertyProvider.ReturnProperty(propertyWrapper);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCommand GetCommand<TCommand>(IBindingContext context, string propertyName) where TCommand : IBaseCommand
        {
            return _commandProvider.GetCommand<TCommand>(context, propertyName);
        }

        public ICommandWrapper GetCommandWrapper(IBindingContext context, CommandBindingData bindingData)
        {
            var commandWrapper = _commandProvider.GetCommandWrapper(context, bindingData);

            if (commandWrapper is ICommandWrapperWithParameter commandWrapperWithParameter)
            {
                commandWrapperWithParameter.SetParameter(bindingData.ElementId, bindingData.ParameterValue);
            }

            return commandWrapper;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RegisterValueConverters(IValueConverter[] converters)
        {
            foreach (var converter in converters)
            {
                switch (converter)
                {
                    case IPropertyValueConverter propertyValueConverter:
                        _propertyProvider.RegisterValueConverter(propertyValueConverter);
                        continue;
                    case IParameterValueConverter parameterValueConverter:
                        _commandProvider.RegisterValueConverter(parameterValueConverter);
                        break;
                }
            }
        }
    }
}