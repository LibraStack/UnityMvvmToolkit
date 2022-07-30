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
    public class BindableVisualElementsCreator<TBindingContext> : IBindableVisualElementsCreator<TBindingContext>
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

        public IBindableElement Create(IBindableUIElement bindableUIElement, TBindingContext bindingContext,
            PropertyInfo propertyInfo)
        {
            return bindableUIElement switch
            {
                BindableLabel label => CreateBindableVisualLabel(label, bindingContext, propertyInfo),
                BindableTextField textField => CreateBindableVisualTextField(textField, bindingContext, propertyInfo),

                BindableButton button when propertyInfo.PropertyType == typeof(ICommand) => new BindableVisualButton(
                    button, CreateReadOnlyProperty<ICommand>(bindingContext, propertyInfo)),

                _ => throw new NotImplementedException(
                    $"Bindable element for {propertyInfo.PropertyType} is not implemented.")
            };
        }

        private IBindableElement CreateBindableVisualLabel(IBindableUIElement bindableUIElement,
            TBindingContext bindingContext, PropertyInfo propertyInfo)
        {
            return CreateBindableElement(typeof(BindableVisualLabel<>), typeof(ReadOnlyProperty<,>), bindableUIElement,
                bindingContext, propertyInfo);
        }

        private IBindableElement CreateBindableVisualTextField(IBindableUIElement bindableUIElement,
            TBindingContext bindingContext, PropertyInfo propertyInfo)
        {
            return CreateBindableElement(typeof(BindableVisualTextField<>), typeof(Property<,>), bindableUIElement,
                bindingContext, propertyInfo);
        }

        private IBindableElement CreateBindableElement(Type elementType, Type propertyType,
            IBindableUIElement bindableUIElement, TBindingContext bindingContext, PropertyInfo sourcePropertyInfo)
        {
            var sourcePropertyType = sourcePropertyInfo.PropertyType;

            // TODO: Cache source properties.
            var genericPropertyType = propertyType.MakeGenericType(typeof(TBindingContext), sourcePropertyType);
            var sourcePropertyInstance = Activator.CreateInstance(genericPropertyType, bindingContext, sourcePropertyInfo);

            var genericElementType = elementType.MakeGenericType(sourcePropertyType);

            return (IBindableElement) Activator.CreateInstance(genericElementType, bindableUIElement, sourcePropertyInstance,
                _valueConverters[sourcePropertyType]);
        }

        private ReadOnlyProperty<TBindingContext, TValueType> CreateReadOnlyProperty<TValueType>(
            TBindingContext bindingContext, PropertyInfo propertyInfo)
        {
            return new ReadOnlyProperty<TBindingContext, TValueType>(bindingContext, propertyInfo);
        }
    }
}