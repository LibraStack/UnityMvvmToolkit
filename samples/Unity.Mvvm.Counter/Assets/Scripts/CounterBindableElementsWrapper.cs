using BindableUIElements;
using BindableUIElementWrappers;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI;

public class CounterBindableElementsWrapper : BindableElementsWrapper
{
    public override IBindableElement Wrap(IBindableUIElement bindableUiElement, IObjectProvider objectProvider)
    {
        return bindableUiElement switch
        {
            BindableRootPage rootPage => new BindableRootPageWrapper(rootPage, objectProvider),
            BindableCounterSlider counterSlider => new BindableCounterSliderWrapper(counterSlider, objectProvider),
            BindableThemeSwitcher themeSwitcher => new BindableThemeSwitcherWrapper(themeSwitcher, objectProvider),
            BindableAnimationLabel animationLabel => new BindableAnimationLabelWrapper(animationLabel, objectProvider),

            _ => base.Wrap(bindableUiElement, objectProvider)
        };
    }
}