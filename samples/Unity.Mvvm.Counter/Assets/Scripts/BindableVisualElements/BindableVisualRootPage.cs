using BindableUIElements;
using Enums;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableVisualElements
{
    public class BindableVisualRootPage : BindableVisualElement
    {
        private readonly BindableRootPage _rootPage;
        private readonly IReadOnlyProperty<ThemeMode> _themeModeProperty;

        public BindableVisualRootPage(BindableRootPage rootPage, IPropertyProvider propertyProvider) 
            : base(propertyProvider)
        {
            _rootPage = rootPage;
            _themeModeProperty = GetReadOnlyProperty<ThemeMode>(rootPage.BindingThemeModePath);
        }

        public override void UpdateValues()
        {
            if (_themeModeProperty != null)
            {
                _rootPage.SetThemeMode(_themeModeProperty.Value);
            }
        }
    }
}