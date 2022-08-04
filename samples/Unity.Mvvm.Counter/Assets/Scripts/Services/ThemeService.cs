using Enums;
using Interfaces.Services;
using UnityEngine;
using UnityEngine.UIElements;

namespace Services
{
    public class ThemeService : MonoBehaviour, IThemeService
    {
        [SerializeField] private PanelSettings _panelSettings;
        [SerializeField] private ThemeStyleSheet _lightTheme;
        [SerializeField] private ThemeStyleSheet _darkTheme;

        public void SetThemeMode(ThemeMode mode)
        {
            var theme = mode == ThemeMode.Light ? _lightTheme : _darkTheme;
            if (_panelSettings.themeStyleSheet != theme)
            {
                _panelSettings.themeStyleSheet = theme;
            }
        }
    }
}