using BindableUIElements;
using Enums;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElementWrappers
{
    public class BindableContentPageWrapper : BindablePropertyElement
    {
        private readonly BindableContentPage _contentPage;
        private readonly IReadOnlyProperty<ThemeMode> _themeModeProperty;

        public BindableContentPageWrapper(BindableContentPage contentPage, IObjectProvider objectProvider)
            : base(objectProvider)
        {
            _contentPage = contentPage;
            _themeModeProperty = GetReadOnlyProperty<ThemeMode>(contentPage.BindingThemeModePath);
        }

        public override void UpdateValues()
        {
            if (_themeModeProperty != null)
            {
                _contentPage.SetThemeMode(_themeModeProperty.Value);
            }
        }
    }
}