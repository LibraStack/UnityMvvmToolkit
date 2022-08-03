using BindableUIElements;
using BindableVisualElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI;

public class CounterBindableElementsCreator : BindableVisualElementsCreator
{
    public override IBindableElement Create(IBindableUIElement bindableUiElement, IPropertyProvider propertyProvider)
    {
        return bindableUiElement switch
        {
            BindableCounterSlider counterSlider => new BindableVisualCounterSlider(counterSlider, propertyProvider),
            BindableThemeSwitcher themeSwitcher => new BindableVisualThemeSwitcher(themeSwitcher, propertyProvider),
            BindableAnimationLabel animationLabel => new BindableVisualAnimationLabel(animationLabel, propertyProvider),

            _ => base.Create(bindableUiElement, propertyProvider)
        };
    }
}