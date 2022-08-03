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
            _panelSettings.themeStyleSheet = mode == ThemeMode.Light ? _lightTheme : _darkTheme;
        }
    }
}