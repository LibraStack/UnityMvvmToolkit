using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.CommandWrappers;

namespace UnityMvvmToolkit.Core.Internal.ObjectProviders
{
    internal class CommandProvider<TBindingContext> : ObjectProvider<TBindingContext>
    {
        private HashSet<IParameterConverter> _parameterConverters;

        internal CommandProvider(TBindingContext bindingContext, IEnumerable<IConverter> converters)
            : base(bindingContext)
        {
            InitializeConverters(converters);
        }

        private void InitializeConverters(IEnumerable<IConverter> converters)
        {
            if (converters == null)
            {
                return;
            }

            _parameterConverters = new HashSet<IParameterConverter>();

            foreach (var converter in converters)
            {
                if (converter is IParameterConverter parameterConverter)
                {
                    _parameterConverters.Add(parameterConverter);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCommand GetCommand<TCommand>(string propertyName) where TCommand : IBaseCommand
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return default;
            }

            if (TryGetInstanceFromCache<TCommand>(propertyName, out var command))
            {
                return command;
            }

            AssurePropertyExist(propertyName, out var propertyInfo);

            if (typeof(TCommand) != propertyInfo.PropertyType)
            {
                throw new InvalidCastException(
                    $"Can not cast the {propertyInfo.PropertyType} command to the {typeof(TCommand)} command.");
            }

            return AddInstanceToCache<TCommand>(propertyName, propertyInfo.GetValue(BindingContext));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ICommandWrapper GetCommandWrapper(string propertyName, ReadOnlyMemory<char> parameterValue,
            ReadOnlyMemory<char> parameterConverterName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return default;
            }

            if (TryGetInstanceFromCache<ICommandWrapper>(propertyName, out var commandWrapper))
            {
                return commandWrapper;
            }

            AssurePropertyExist(propertyName, out var propertyInfo);

            var propertyType = propertyInfo.PropertyType;
            if (propertyType.GetInterface(nameof(IBaseCommand)) == null)
            {
                throw new InvalidCastException(
                    $"Can not cast the {propertyInfo.PropertyType} command to the {typeof(IBaseCommand)} command."); // TODO: Conditional?
            }

            if (propertyType == typeof(ICommand) || propertyType.GetInterface(nameof(ICommand)) != null)
            {
                return AddInstanceToCache<ICommandWrapper>(propertyName,
                    new CommandWrapper((ICommand) propertyInfo.GetValue(BindingContext)));
            }

            if (propertyType.IsGenericType == false)
            {
                throw new InvalidCastException(
                    $"Can not cast the {propertyInfo.PropertyType} command to the {typeof(ICommand<>)} command.");
            }

            object[] args;
            Type genericCommandWrapperType;

            var command = propertyInfo.GetValue(BindingContext);
            var commandValueType = propertyType.GenericTypeArguments[0];

            if (parameterValue.IsEmptyOrWhiteSpace())
            {
                args = new[]
                {
                    command,
                    default
                };

                genericCommandWrapperType = typeof(CommandWrapper<>).MakeGenericType(commandValueType);
            }
            else if (commandValueType == typeof(ReadOnlyMemory<char>))
            {
                args = new[]
                {
                    command,
                    parameterValue
                };

                genericCommandWrapperType = typeof(CommandWrapper<>).MakeGenericType(commandValueType);
            }
            else
            {
                args = new[]
                {
                    command,
                    parameterValue,
                    GetParameterConverter(commandValueType, parameterConverterName.Span)
                };

                genericCommandWrapperType =
                    typeof(CommandWrapperWithConverter<>).MakeGenericType(commandValueType);
            }

            return AddInstanceToCache<ICommandWrapper>(propertyName,
                Activator.CreateInstance(genericCommandWrapperType, args));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IParameterConverter GetParameterConverter(Type targetType, ReadOnlySpan<char> converterName)
        {
            if (_parameterConverters == null)
            {
                throw new NullReferenceException(nameof(_parameterConverters));
            }

            var parameterConverter = converterName.IsEmpty
                ? GetConverter(targetType)
                : GetConverter(targetType, converterName);

            if (parameterConverter == null)
            {
                throw new NullReferenceException($"Parameter converter for {targetType} type is missing");
            }

            return parameterConverter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IParameterConverter GetConverter(Type targetType)
        {
            return _parameterConverters.FirstOrDefault(converter => converter.TargetType == targetType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IParameterConverter GetConverter(Type targetType, ReadOnlySpan<char> converterName)
        {
            foreach (var converter in _parameterConverters)
            {
                if (converter.TargetType == targetType &&
                    converterName.SequenceEqual(converter.Name))
                {
                    return converter;
                }
            }

            throw new NullReferenceException($"Parameter converter '{converterName.ToString()}' not found.");
        }
    }
}