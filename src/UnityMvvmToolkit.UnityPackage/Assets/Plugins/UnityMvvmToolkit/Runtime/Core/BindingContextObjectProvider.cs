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
        private readonly PropertyProvider _propertyProvider;

        public BindingContextObjectProvider(IValueConverter[] converters)
        {
            _propertyProvider = new PropertyProvider(new BindingContextMembersProvider());

            RegisterValueConverters(converters);
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

        public IProperty<TValueType> RentProperty<TValueType>(IBindingContext context, PropertyBindingData bindingData)
        {
            return _propertyProvider.GetProperty<TValueType>(context, bindingData);
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
            return _propertyProvider.GetReadOnlyProperty<TValueType>(context, bindingData);
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
            return _propertyProvider.GetCommand<TCommand>(context, propertyName);
        }

        public ICommandWrapper GetCommandWrapper(IBindingContext context, CommandBindingData bindingData)
        {
            // var commandWrapper = _commandProvider.GetCommandWrapper(context, bindingData);
            //
            // if (commandWrapper is ICommandWrapperWithParameter commandWrapperWithParameter)
            // {
            //     commandWrapperWithParameter.SetParameter(bindingData.ElementId, bindingData.ParameterValue);
            // }

            return default;
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
                    // case IParameterValueConverter parameterValueConverter:
                    //     _commandProvider.RegisterValueConverter(parameterValueConverter);
                    //     break;
                }
            }
        }
    }
}