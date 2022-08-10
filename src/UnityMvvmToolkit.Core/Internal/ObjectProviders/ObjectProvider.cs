using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityMvvmToolkit.Core.Interfaces;

namespace UnityMvvmToolkit.Core.Internal.ObjectProviders
{
    internal abstract class ObjectProvider<TBindingContext>
    {
        private readonly TBindingContext _bindingContext;
        private readonly Dictionary<(string, Type), object> _cachedInstances;

        internal ObjectProvider(TBindingContext bindingContext)
        {
            _bindingContext = bindingContext;
            _cachedInstances = new Dictionary<(string, Type), object>();
        }

        protected TBindingContext BindingContext => _bindingContext;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected HashSet<T> GetValueConverters<T>(IEnumerable<IValueConverter> converters) where T : IValueConverter
        {
            var result = new HashSet<T>();

            foreach (var converter in converters)
            {
                if (converter is T valueConverter)
                {
                    result.Add(valueConverter);
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void AssurePropertyExist(string propertyName, out PropertyInfo propertyInfo)
        {
            propertyInfo = _bindingContext.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new NullReferenceException($"Property '{propertyName}' not found.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T AddInstanceToCache<T>(string propertyName, object instance)
        {
            _cachedInstances.Add((propertyName, typeof(T)), instance);
            return (T) instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool TryGetInstanceFromCache<T>(string propertyName, out T instance)
        {
            if (_cachedInstances.TryGetValue((propertyName, typeof(T)), out var cachedInstance))
            {
                instance = (T) cachedInstance;
                return true;
            }

            instance = default;
            return false;
        }
    }
}