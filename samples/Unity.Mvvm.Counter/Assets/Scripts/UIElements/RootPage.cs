using Enums;
using Interfaces.Services;
using UnityEngine.UIElements;

namespace UIElements
{
    public class RootPage : VisualElement
    {
        private IThemeService _themeService;

        public void Initialize(IThemeService themeService)
        {
            _themeService = themeService;
        }

        public void SetThemeMode(ThemeMode themeMode)
        {
            _themeService?.SetThemeMode(themeMode);
        }

        public new class UxmlFactory : UxmlFactory<RootPage, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }
    }
}