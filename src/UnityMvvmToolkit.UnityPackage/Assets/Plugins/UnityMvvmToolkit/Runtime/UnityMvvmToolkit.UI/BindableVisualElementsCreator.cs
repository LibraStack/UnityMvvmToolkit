using System;
using System.Collections.Generic;
using System.Reflection;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Common.Properties;
using UnityMvvmToolkit.Common.ValueConverters;
using UnityMvvmToolkit.UI.BindableUIElements;
using UnityMvvmToolkit.UI.BindableVisualElements;
using UnityMvvmToolkit.UI.Interfaces;

namespace UnityMvvmToolkit.UI
{
    public class BindableVisualElementsCreator : IBindableVisualElementsCreator
    {
        private readonly Dictionary<Type, IValueConverter> _valueConverters;

        public BindableVisualElementsCreator()
        {
            _valueConverters = new Dictionary<Type, IValueConverter>
            {
                { typeof(int), new IntToStrConverter() },
                { typeof(float), new FloatToStrConverter() },
                { typeof(string), new DefaultConverter() }
            };
        }

        public virtual IBindableElement Create<TBindingContext>(TBindingContext bindingContext,
            IBindableUIElement bindableUIElement, PropertyInfo propertyInfo)
        {
            return bindableUIElement switch
            {
                BindableLabel label => CreateBindableVisualLabel(bindingContext, label, propertyInfo),
                BindableTextField textField => CreateBindableVisualTextField(bindingContext, textField, propertyInfo),

                BindableButton button when propertyInfo.PropertyType == typeof(ICommand) => new BindableVisualButton(
                    button, CreateReadOnlyProperty<TBindingContext, ICommand>(bindingContext, propertyInfo)),

                _ => throw new NotImplementedException(
                    $"Bindable element for {propertyInfo.PropertyType} is not implemented.")
            };
        }

        private IBindableElement CreateBindableVisualLabel<TBindingContext>(TBindingContext bindingContext,
            IBindableUIElement bindableLabel, PropertyInfo propertyInfo)
        {
            return CreateBindableElement(typeof(BindableVisualLabel<>), typeof(ReadOnlyProperty<,>), bindingContext,
                bindableLabel, propertyInfo);
        }

        private IBindableElement CreateBindableVisualTextField<TBindingContext>(TBindingContext bindingContext,
            IBindableUIElement bindableTextField, PropertyInfo propertyInfo)
        {
            return CreateBindableElement(typeof(BindableVisualTextField<>), typeof(Property<,>), bindingContext,
                bindableTextField, propertyInfo);
        }

        protected IBindableElement CreateBindableElement<TBindingContext>(Type elementType, Type propertyType,
            TBindingContext bindingContext, IBindableUIElement bindableUIElement, PropertyInfo sourcePropertyInfo)
        {
            var sourcePropertyType = sourcePropertyInfo.PropertyType;

            // TODO: Cache source properties.
            var genericPropertyType = propertyType.MakeGenericType(typeof(TBindingContext), sourcePropertyType);
            var sourcePropertyInstance =
                Activator.CreateInstance(genericPropertyType, bindingContext, sourcePropertyInfo);

            var genericElementType = elementType.MakeGenericType(sourcePropertyType);

            return (IBindableElement) Activator.CreateInstance(genericElementType, bindableUIElement,
                sourcePropertyInstance,
                _valueConverters[sourcePropertyType]);
        }

        private ReadOnlyProperty<TBindingContext, TValueType> CreateReadOnlyProperty<TBindingContext, TValueType>(
            TBindingContext bindingContext, PropertyInfo propertyInfo)
        {
            return new ReadOnlyProperty<TBindingContext, TValueType>(bindingContext, propertyInfo);
        }
    }
}