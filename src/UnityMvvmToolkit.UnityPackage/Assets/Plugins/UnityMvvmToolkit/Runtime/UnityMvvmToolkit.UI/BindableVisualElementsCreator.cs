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
    public class BindableVisualElementsCreator<TDataContext> : IBindableVisualElementsCreator<TDataContext>
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

        public IBindableElement Create(IBindableUIElement bindableUIElement, TDataContext dataContext,
            PropertyInfo propertyInfo)
        {
            return bindableUIElement switch
            {
                BindableLabel label => CreateBindableVisualLabel(label, dataContext, propertyInfo),
                BindableTextField textField => CreateBindableVisualTextField(textField, dataContext, propertyInfo),

                BindableButton button when propertyInfo.PropertyType == typeof(ICommand) => new BindableVisualButton(
                    button, CreateReadOnlyProperty<ICommand>(dataContext, propertyInfo)),

                _ => throw new NotImplementedException(
                    $"Bindable element for {propertyInfo.PropertyType} is not implemented.")
            };
        }

        private IBindableElement CreateBindableVisualLabel(IBindableUIElement bindableUIElement, TDataContext dataContext,
            PropertyInfo propertyInfo)
        {
            return CreateBindableElement(typeof(BindableVisualLabel<>), typeof(ReadOnlyProperty<,>), bindableUIElement,
                dataContext, propertyInfo);
        }

        private IBindableElement CreateBindableVisualTextField(IBindableUIElement bindableUIElement, TDataContext dataContext,
            PropertyInfo propertyInfo)
        {
            return CreateBindableElement(typeof(BindableVisualTextField<>), typeof(Property<,>), bindableUIElement,
                dataContext, propertyInfo);
        }

        private IBindableElement CreateBindableElement(Type elementType, Type propertyType,
            IBindableUIElement bindableUIElement, TDataContext dataContext, PropertyInfo sourcePropertyInfo)
        {
            var sourcePropertyType = sourcePropertyInfo.PropertyType;

            // TODO: Cache source properties.
            var genericPropertyType = propertyType.MakeGenericType(typeof(TDataContext), sourcePropertyType);
            var sourcePropertyInstance = Activator.CreateInstance(genericPropertyType, dataContext, sourcePropertyInfo);

            var genericElementType = elementType.MakeGenericType(sourcePropertyType);

            return (IBindableElement) Activator.CreateInstance(genericElementType, bindableUIElement, sourcePropertyInstance,
                _valueConverters[sourcePropertyType]);
        }

        private ReadOnlyProperty<TDataContext, TValueType> CreateReadOnlyProperty<TValueType>(TDataContext dataContext,
            PropertyInfo propertyInfo)
        {
            return new ReadOnlyProperty<TDataContext, TValueType>(dataContext, propertyInfo);
        }
    }
}