using Enums;
using UIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Extensions;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElements
{
    public partial class BindableContentPage : ContentPage, IBindableElement
    {
        private PropertyBindingData _propertyBindingData;
        private IReadOnlyProperty<ThemeMode> _themeModeProperty;

        public void SetBindingContext(IBindingContext context, IObjectProvider objectProvider)
        {
            _propertyBindingData ??= BindingThemeModePath.ToPropertyBindingData();

            _themeModeProperty = objectProvider.RentReadOnlyProperty<ThemeMode>(context, _propertyBindingData);
            _themeModeProperty.ValueChanged += OnPropertyValueChanged;

            SetThemeMode(_themeModeProperty.Value);
        }

        public void ResetBindingContext(IObjectProvider objectProvider)
        {
            if (_themeModeProperty == null)
            {
                return;
            }

            objectProvider.ReturnReadOnlyProperty(_themeModeProperty);

            _themeModeProperty.ValueChanged -= OnPropertyValueChanged;
            _themeModeProperty = null;
        }

        private void OnPropertyValueChanged(object sender, ThemeMode themeMode)
        {
            SetThemeMode(themeMode);
        }
    }
}