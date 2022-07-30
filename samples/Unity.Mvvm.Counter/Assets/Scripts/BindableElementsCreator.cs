using System.Reflection;
using BindableUIElements;
using BindableVisualElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.Common.Properties;
using UnityMvvmToolkit.UI;

public class BindableElementsCreator<TBindingContext> : BindableVisualElementsCreator<TBindingContext>
{
    public override IBindableElement Create(IBindableUIElement bindableUIElement, TBindingContext bindingContext,
        PropertyInfo propertyInfo)
    {
        return bindableUIElement switch
        {
            BindableThemeSwitcher themeSwitcher => CreateBindableVisualThemeSwitcher(themeSwitcher, bindingContext,
                propertyInfo),
            BindableAnimationLabel animationLabel => CreateBindableVisualAnimationLabel(animationLabel, bindingContext,
                propertyInfo),
            BindableCounterSlider counterSlider when propertyInfo.PropertyType == typeof(ICommand) =>
                CreateBindableVisualCounterSlider(counterSlider, bindingContext, propertyInfo),

            _ => base.Create(bindableUIElement, bindingContext, propertyInfo)
        };
    }

    private IBindableElement CreateBindableVisualCounterSlider(BindableCounterSlider counterSlider,
        TBindingContext bindingContext, PropertyInfo propertyInfo)
    {
        return new BindableVisualCounterSlider(counterSlider,
            new ReadOnlyProperty<TBindingContext, ICommand>(bindingContext, propertyInfo));
    }

    private IBindableElement CreateBindableVisualAnimationLabel(BindableAnimationLabel animationLabel,
        TBindingContext bindingContext, PropertyInfo propertyInfo)
    {
        return CreateBindableElement(typeof(BindableVisualAnimationLabel<>), typeof(ReadOnlyProperty<,>),
            animationLabel, bindingContext, propertyInfo);
    }

    private IBindableElement CreateBindableVisualThemeSwitcher(BindableThemeSwitcher themeSwitcher,
        TBindingContext bindingContext, PropertyInfo propertyInfo)
    {
        return new BindableVisualThemeSwitcher(themeSwitcher,
            new Property<TBindingContext, bool>(bindingContext, propertyInfo));
    }
}