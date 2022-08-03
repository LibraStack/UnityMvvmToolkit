using System.Reflection;
using BindableUIElements;
using BindableVisualElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Common.Properties;
using UnityMvvmToolkit.UI;

public class BindableElementsCreator : BindableVisualElementsCreator
{
    public override IBindableElement Create<TBindingContext>(TBindingContext bindingContext,
        IBindableUIElement bindableUIElement, PropertyInfo propertyInfo)
    {
        return bindableUIElement switch
        {
            BindableThemeSwitcher themeSwitcher => CreateBindableVisualThemeSwitcher(bindingContext, themeSwitcher,
                propertyInfo),
            BindableAnimationLabel animationLabel => CreateBindableVisualAnimationLabel(bindingContext, animationLabel,
                propertyInfo),
            BindableCounterSlider counterSlider when propertyInfo.PropertyType == typeof(ICommand) =>
                CreateBindableVisualCounterSlider(bindingContext, counterSlider, propertyInfo),

            _ => base.Create(bindingContext, bindableUIElement, propertyInfo)
        };
    }

    private IBindableElement CreateBindableVisualCounterSlider<TBindingContext>(TBindingContext bindingContext,
        BindableCounterSlider counterSlider, PropertyInfo propertyInfo)
    {
        return new BindableVisualCounterSlider(counterSlider,
            new ReadOnlyProperty<TBindingContext, ICommand>(bindingContext, propertyInfo));
    }

    private IBindableElement CreateBindableVisualAnimationLabel<TBindingContext>(TBindingContext bindingContext,
        BindableAnimationLabel animationLabel, PropertyInfo propertyInfo)
    {
        return CreateBindableElement(typeof(BindableVisualAnimationLabel<>), typeof(ReadOnlyProperty<,>),
            bindingContext, animationLabel, propertyInfo);
    }

    private IBindableElement CreateBindableVisualThemeSwitcher<TBindingContext>(TBindingContext bindingContext,
        BindableThemeSwitcher themeSwitcher, PropertyInfo propertyInfo)
    {
        return new BindableVisualThemeSwitcher(themeSwitcher,
            new Property<TBindingContext, bool>(bindingContext, propertyInfo));
    }
}