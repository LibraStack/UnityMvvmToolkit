using Enums;
using Interfaces.Services;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableUIElements
{
    public class BindableRootPage : VisualElement, IBindableUIElement
    {
        private IThemeService _themeService;

        public string BindingThemeModePath { get; set; }

        public void Initialize(IThemeService themeService)
        {
            _themeService = themeService;
        }

        public void SetThemeMode(ThemeMode themeMode)
        {
            _themeService?.SetThemeMode(themeMode);
        }

        public new class UxmlFactory : UxmlFactory<BindableRootPage, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _bindingThemeModeAttribute = new()
                { name = "binding-theme-mode-path", defaultValue = "" };

            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext context)
            {
                base.Init(visualElement, bag, context);
                ((BindableRootPage) visualElement).BindingThemeModePath =
                    _bindingThemeModeAttribute.GetValueFromBag(bag, context);
            }
        }
    }
}