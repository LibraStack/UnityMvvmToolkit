using Enums;

namespace Interfaces
{
    public interface IThemeService
    {
        bool IsDarkMode { get; }
        void SetThemeMode(ThemeMode mode);
    }
}