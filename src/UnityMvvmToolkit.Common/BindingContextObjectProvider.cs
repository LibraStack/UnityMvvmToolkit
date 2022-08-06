using System;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Common.Internal.ObjectProviders;

namespace UnityMvvmToolkit.Common
{
    public class BindingContextObjectProvider<TBindingContext> : IObjectProvider
    {
        private readonly CommandProvider<TBindingContext> _commandProvider;
        private readonly PropertyProvider<TBindingContext> _propertyProvider;

        public BindingContextObjectProvider(TBindingContext bindingContext, IConverter[] converters)
        {
            _commandProvider = new CommandProvider<TBindingContext>(bindingContext, converters);
            _propertyProvider = new PropertyProvider<TBindingContext>(bindingContext, converters);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCommand GetCommand<TCommand>(string propertyName) where TCommand : IBaseCommand
        {
            return _commandProvider.GetCommand<TCommand>(propertyName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ICommandWrapper GetCommandWrapper(string propertyName, ReadOnlyMemory<char> parameterValue,
            ReadOnlyMemory<char> parameterConverterName)
        {
            return _commandProvider.GetCommandWrapper(propertyName, parameterValue, parameterConverterName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IProperty<TValueType> GetProperty<TValueType>(string propertyName, ReadOnlyMemory<char> converterName)
        {
            return _propertyProvider.GetProperty<TValueType>(propertyName, converterName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyProperty<TValueType> GetReadOnlyProperty<TValueType>(string propertyName,
            ReadOnlyMemory<char> converterName)
        {
            return _propertyProvider.GetReadOnlyProperty<TValueType>(propertyName, converterName);
        }
    }
}