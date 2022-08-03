using BindableUIElements;
using BindableVisualElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI;

public class BindableElementsCreator : BindableVisualElementsCreator
{
    public override IBindableElement Create(IBindableUIElement bindableUiElement, IPropertyProvider propertyProvider)
    {
        return bindableUiElement switch
        {
            BindableThemeSwitcher themeSwitcher => new BindableVisualThemeSwitcher(themeSwitcher, propertyProvider),
            BindableAnimationLabel animationLabel => new BindableVisualAnimationLabel(animationLabel, propertyProvider),
            BindableCounterSlider counterSlider => new BindableVisualCounterSlider(counterSlider, propertyProvider),

            _ => base.Create(bindableUiElement, propertyProvider)
        };
    }
}