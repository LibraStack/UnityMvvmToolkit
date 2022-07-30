using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace Services
{
    public class ThemeService : MonoBehaviour, IThemeService
    {
        [SerializeField] private PanelSettings _panelSettings;
        [SerializeField] private ThemeStyleSheet _lightTheme;
        [SerializeField] private ThemeStyleSheet _darkTheme;

        public bool IsDarkMode => _panelSettings.themeStyleSheet.name == _darkTheme.name;

        public void SetThemeMode(ThemeMode mode)
        {
            _panelSettings.themeStyleSheet = mode == ThemeMode.Light ? _lightTheme : _darkTheme;
        }
    }
}