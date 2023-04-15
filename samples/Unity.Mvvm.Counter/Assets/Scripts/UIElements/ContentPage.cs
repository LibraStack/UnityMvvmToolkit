using Enums;
using Interfaces.Services;
using UnityEngine.UIElements;

namespace UIElements
{
    public class ContentPage : VisualElement
    {
        private IThemeService _themeService;

        public void Initialize(IThemeService themeService)
        {
            _themeService = themeService;
        }

        protected void SetThemeMode(ThemeMode themeMode)
        {
            _themeService?.SetThemeMode(themeMode);
        }

        public new class UxmlFactory : UxmlFactory<ContentPage, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }
    }
}