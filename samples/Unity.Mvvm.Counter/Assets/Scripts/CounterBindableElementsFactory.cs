using BindableUIElements;
using BindableUIElementWrappers;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK;

public class CounterBindableElementsFactory : BindableElementsFactory
{
    public override IBindableElement Create(IBindableUIElement bindableUiElement, IObjectProvider objectProvider)
    {
        return bindableUiElement switch
        {
            BindableContentPage contentPage => new BindableContentPageWrapper(contentPage, objectProvider),
            BindableCounterSlider counterSlider => new BindableCounterSliderWrapper(counterSlider, objectProvider),
            BindableThemeSwitcher themeSwitcher => new BindableThemeSwitcherWrapper(themeSwitcher, objectProvider),
            BindableAnimationLabel animationLabel => new BindableAnimationLabelWrapper(animationLabel, objectProvider),

            _ => base.Create(bindableUiElement, objectProvider)
        };
    }
}