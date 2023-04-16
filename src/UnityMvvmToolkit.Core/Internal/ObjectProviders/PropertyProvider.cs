using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.BindingContextObjectWrappers.PropertyWrappers;
using UnityMvvmToolkit.Core.Internal.Helpers;
using UnityMvvmToolkit.Core.Internal.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.ObjectProviders
{
    internal class PropertyProvider
    {
        private readonly BindingContextMembersProvider _membersProvider;

        private readonly Dictionary<int, MemberInfo> _memberInfos;
        private readonly Dictionary<int, Queue<IPropertyWrapper>> _propertiesWithConverter;

        private readonly HashSet<int> _initializedBindingContexts;
        private readonly HashSet<IPropertyValueConverter> _propertyValueConverters;

        internal PropertyProvider(BindingContextMembersProvider membersProvider)
        {
            _membersProvider = membersProvider;
            _memberInfos = new Dictionary<int, MemberInfo>();
            _propertiesWithConverter = new Dictionary<int, Queue<IPropertyWrapper>>();

            _initializedBindingContexts = new HashSet<int>();
            _propertyValueConverters = new HashSet<IPropertyValueConverter>();
        }

        public void RegisterValueConverter(IPropertyValueConverter converter)
        {
            _propertyValueConverters.Add(converter);
        }

        public void WarmupBindingContext(Type bindingContextType)
        {
            if (_initializedBindingContexts.Add(bindingContextType.GetHashCode()))
            {
                _membersProvider.GetBindingContextMembers(bindingContextType, _memberInfos);
            }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCommand GetCommand<TCommand>(IBindingContext context, string propertyName) where TCommand : IBaseCommand
        {
            var contextType = context.GetType();

            if (IsBindingContextInitialized(contextType) == false)
            {
                WarmupBindingContext(contextType);
            }

            var memberHash = HashCodeHelper.GetMemberHashCode(contextType, propertyName);

            if (_memberInfos.TryGetValue(memberHash, out var memberInfo) == false ||
                memberInfo.MemberType != MemberTypes.Property)
            {
                throw new InvalidOperationException($"Property '{propertyName}' not found.");
            }

            var propertyInfo = (PropertyInfo) memberInfo;

            if (propertyInfo.PropertyType != typeof(TCommand))
            {
                throw new InvalidCastException(
                    $"Can not cast the {propertyInfo.PropertyType} command to the {typeof(TCommand)} command.");
            }

            return (TCommand) propertyInfo.GetValue(context);
        }

        public void ReturnProperty(IPropertyWrapper propertyWrapper)
        {
            propertyWrapper.Reset();
            _propertiesWithConverter[propertyWrapper.GetHashCode()].Enqueue(propertyWrapper);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TProperty GetProperty<TProperty, TValueType>(IBindingContext context, PropertyBindingData bindingData)
        {
            var contextType = context.GetType();

            if (IsBindingContextInitialized(contextType) == false)
            {
                WarmupBindingContext(contextType);
            }

            var memberHash = HashCodeHelper.GetMemberHashCode(contextType, bindingData.PropertyName);

            if (_memberInfos.TryGetValue(memberHash, out var memberInfo) == false)
            {
                throw new InvalidOperationException($"Property '{bindingData.PropertyName}' not found.");
            }

            var memberValue = GetMemberValue(context, memberInfo, out var memberValueType);

            if (memberValueType == typeof(TValueType))
            {
                return (TProperty) memberValue;
            }

            var propertyWrapperHashCode = IPropertyWrapper.GenerateHashCode(typeof(TValueType), memberValueType);

            if (_propertiesWithConverter.TryGetValue(propertyWrapperHashCode, out var propertyWrappers))
            {
                if (propertyWrappers.Count > 0)
                {
                    return (TProperty) propertyWrappers
                        .Dequeue()
                        .SetProperty(memberValue);
                }
            }
            else
            {
                _propertiesWithConverter.Add(propertyWrapperHashCode, new Queue<IPropertyWrapper>());
            }

            var args = new object[]
            {
                GetValueConverter<TValueType>(memberValueType, bindingData.ConverterName)
            };

            var genericPropertyType =
                typeof(PropertyWithConverter<,>).MakeGenericType(typeof(TValueType), memberValueType);

            return (TProperty) ((IPropertyWrapper) Activator.CreateInstance(genericPropertyType, args))
                .SetProperty(memberValue);
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
        private IPropertyValueConverter GetValueConverter<TValueType>(Type sourceType, string converterName)
        {
            var valueConverter = string.IsNullOrEmpty(converterName)
                ? GetConverter(sourceType, typeof(TValueType))
                : GetConverter(sourceType, typeof(TValueType), converterName);

            if (valueConverter != null)
            {
                return valueConverter;
            }

            if (string.IsNullOrEmpty(converterName))
            {
                throw new NullReferenceException($"Converter is missing: From {sourceType} To {typeof(TValueType)}");
            }

            throw new NullReferenceException($"Converter '{converterName}' not found.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IPropertyValueConverter GetConverter(Type sourceType, Type targetType)
        {
            foreach (var converter in _propertyValueConverters)
            {
                if (converter.SourceType == sourceType && converter.TargetType == targetType)
                {
                    return converter;
                }
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IPropertyValueConverter GetConverter(Type sourceType, Type targetType, string converterName)
        {
            foreach (var converter in _propertyValueConverters)
            {
                if (converter.SourceType == sourceType &&
                    converter.TargetType == targetType &&
                    converter.Name == converterName)
                {
                    return converter;
                }
            }

            return null;
        }
    }
}