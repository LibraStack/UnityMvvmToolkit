using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Converters.ParameterValueConverters;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.Core.Internal.Helpers;

namespace UnityMvvmToolkit.Core.Internal.ObjectHandlers
{
    internal sealed class ValueConverterHandler : IDisposable
    {
        private readonly Dictionary<int, IValueConverter> _valueConvertersByHash;

        public ValueConverterHandler(IValueConverter[] valueConverters)
        {
            _valueConvertersByHash = new Dictionary<int, IValueConverter>();

            RegisterValueConverters(valueConverters);
        }

        public IValueConverter GetValueConverterById(int converterId)
        {
            return _valueConvertersByHash[converterId]; // TODO: throw clear exception.
        }

        public IValueConverter GetValueConverterByType(Type converterType)
        {
            return _valueConvertersByHash[converterType.GetHashCode()]; // TODO: throw clear exception.
        }

        public void Dispose()
        {
            _valueConvertersByHash.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RegisterValueConverters(IValueConverter[] converters)
        {
            var convertersSpan = converters.AsSpan();

            for (var i = 0; i < convertersSpan.Length; i++)
            {
                RegisterValueConverter(convertersSpan[i]);
            }

            if (_valueConvertersByHash.ContainsKey(typeof(ParameterToStrConverter).GetHashCode()) == false)
            {
                RegisterValueConverter(new ParameterToStrConverter());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RegisterValueConverter(IValueConverter valueConverter)
        {
            int converterHashByType;
            int converterHashByName;
            var converterTypeHash = valueConverter.GetType().GetHashCode();

            switch (valueConverter)
            {
                case IPropertyValueConverter converter:
                    converterHashByType = HashCodeHelper.GetPropertyConverterHashCode(converter);
                    converterHashByName = HashCodeHelper.GetPropertyConverterHashCode(converter, converter.Name);
                    break;
                case IParameterValueConverter converter:
                    converterHashByType = HashCodeHelper.GetParameterConverterHashCode(converter);
                    converterHashByName = HashCodeHelper.GetParameterConverterHashCode(converter, converter.Name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _valueConvertersByHash.Add(converterTypeHash, valueConverter);
            _valueConvertersByHash.Add(converterHashByType, valueConverter);
            _valueConvertersByHash.Add(converterHashByName, valueConverter);
        }
    }
}