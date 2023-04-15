using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.CommandWrappers;
using UnityMvvmToolkit.Core.Internal.Helpers;

namespace UnityMvvmToolkit.Core.Internal.ObjectProviders
{
    internal class CommandProvider
    {
        /// <summary>
        /// Stores a PropertyInfo.
        /// Key is a combination of the IBindingContext and the Property name hash codes.
        /// </summary>
        private readonly Dictionary<int, PropertyInfo> _properties;

        /// <summary>
        /// Stores an array of PropertyInfo for each IBindingContext.
        /// Key is the IBindingContext type.
        /// </summary>
        private readonly Dictionary<Type, PropertyInfo[]> _bindingContextProperties;

        /// <summary>
        /// Stores a queue of ICommandWrapper instances.
        /// Key is the ICommandWrapper hash code.
        /// </summary>
        private readonly Dictionary<int, Queue<ICommandWrapper>> _commandWrappers;

        private readonly HashSet<IParameterValueConverter> _parameterConverters;

        internal CommandProvider()
        {
            _properties = new Dictionary<int, PropertyInfo>();
            _commandWrappers = new Dictionary<int, Queue<ICommandWrapper>>();
            _bindingContextProperties = new Dictionary<Type, PropertyInfo[]>();

            _parameterConverters = new HashSet<IParameterValueConverter>();
        }

        public void RegisterValueConverter(IParameterValueConverter converter)
        {
            _parameterConverters.Add(converter);
        }

        public void WarmupBindingContext(Type bindingContextType, PropertyInfo[] properties)
        {
            var propertiesSpan = properties.AsSpan();

            for (var i = 0; i < propertiesSpan.Length; i++)
            {
                var propertyInfo = propertiesSpan[i];

                if (propertyInfo.PropertyType.GetInterface(nameof(IBaseCommand)) == null)
                {
                    continue;
                }

                // TODO: Test.
                var bindingContextTypeHash = bindingContextType.GetHashCode();
                var propertyNameHash = propertyInfo.Name.GetHashCode();

                _properties.Add(HashCodeHelper.CombineHashCode(bindingContextTypeHash, propertyNameHash),
                    propertyInfo);
            }

            _bindingContextProperties.Add(bindingContextType, properties);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCommand GetCommand<TCommand>(IBindingContext context, string propertyName) where TCommand : IBaseCommand
        {
            var propertyInfo = GetPropertyInfo(context, propertyName);

            if (propertyInfo.PropertyType != typeof(TCommand))
            {
                throw new InvalidCastException(
                    $"Can not cast the {propertyInfo.PropertyType} command to the {typeof(TCommand)} command.");
            }

            return (TCommand) propertyInfo.GetValue(context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ICommandWrapper GetCommandWrapper(IBindingContext context, CommandBindingData bindingData)
        {
            var propertyName = bindingData.PropertyName;
            var propertyInfo = GetPropertyInfo(context, propertyName);

            var propertyType = propertyInfo.PropertyType;
            if (propertyType.GetInterface(nameof(IBaseCommand)) == null)
            {
                throw new InvalidCastException(
                    $"Can not cast the {propertyInfo.PropertyType} command to the {typeof(IBaseCommand)} command."); // TODO: Conditional?
            }

            return default;
            // if (propertyType == typeof(ICommand) || propertyType.GetInterface(nameof(ICommand)) != null)
            // {
            //     return AddInstanceToCache<ICommandWrapper>(propertyName,
            //         new CommandWrapper((ICommand) propertyInfo.GetValue(null)));
            // }
            //
            // if (propertyType.IsGenericType == false)
            // {
            //     throw new InvalidCastException(
            //         $"Can not cast the {propertyInfo.PropertyType} command to the {typeof(ICommand<>)} command.");
            // }
            //
            // var commandValueType = propertyType.GenericTypeArguments[0];
            // if (commandValueType == typeof(string))
            // {
            //     return AddInstanceToCache<ICommandWrapper>(propertyName,
            //         new CommandWrapperWithoutConverter(
            //             (ICommand<string>) propertyInfo.GetValue(null)));
            // }
            //
            // var args = new[]
            // {
            //     propertyInfo.GetValue(null),
            //     GetParameterConverter(commandValueType, parameterConverterName.Span)
            // };
            //
            // var genericCommandWrapperType = typeof(CommandWrapperWithConverter<>).MakeGenericType(commandValueType);
            //
            // return AddInstanceToCache<ICommandWrapper>(propertyName,
            //     Activator.CreateInstance(genericCommandWrapperType, args));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private PropertyInfo GetPropertyInfo(IBindingContext context, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new NullReferenceException(propertyName);
            }

            var bindingContextType = context.GetType();

            var bindingContextTypeHash = bindingContextType.GetHashCode();
            var propertyNameHash = propertyName.GetHashCode();

            var propertyKey = HashCodeHelper.CombineHashCode(bindingContextTypeHash, propertyNameHash);

            if (_properties.TryGetValue(propertyKey, out var propertyInfo))
            {
                return propertyInfo;
            }

            propertyInfo = GetPropertyInfo(bindingContextType, propertyName);
            _properties.Add(propertyKey, propertyInfo);

            return propertyInfo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private PropertyInfo GetPropertyInfo(Type bindingContextType, string propertyName)
        {
            if (_bindingContextProperties.TryGetValue(bindingContextType, out var bindingContextProperties) == false)
            {
                bindingContextProperties = bindingContextType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public);

                _bindingContextProperties.Add(bindingContextType, bindingContextProperties);
            }

            var propertiesSpan = bindingContextProperties.AsSpan();

            for (var i = 0; i < propertiesSpan.Length; i++)
            {
                var propertyInfo = propertiesSpan[i];

                if (propertyInfo.Name == propertyName)
                {
                    return propertyInfo;
                }
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IParameterValueConverter GetParameterConverter(Type targetType, ReadOnlySpan<char> converterName)
        {
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
        private IParameterValueConverter GetConverter(Type targetType)
        {
            return _parameterConverters.FirstOrDefault(converter => converter.TargetType == targetType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IParameterValueConverter GetConverter(Type targetType, ReadOnlySpan<char> converterName)
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