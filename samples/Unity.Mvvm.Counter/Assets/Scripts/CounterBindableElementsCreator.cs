using BindableUIElements;
using BindableVisualElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI;

public class CounterBindableElementsCreator : BindableVisualElementsCreator
{
    public override IBindableElement Create(IBindableUIElement bindableUiElement, IObjectProvider objectProvider)
    {
        return bindableUiElement switch
        {
            BindableRootPage rootPage => new BindableVisualRootPage(rootPage, objectProvider),
            BindableCounterSlider counterSlider => new BindableVisualCounterSlider(counterSlider, objectProvider),
            BindableThemeSwitcher themeSwitcher => new BindableVisualThemeSwitcher(themeSwitcher, objectProvider),
            BindableAnimationLabel animationLabel => new BindableVisualAnimationLabel(animationLabel, objectProvider),

            _ => base.Create(bindableUiElement, objectProvider)
        };
    }
}