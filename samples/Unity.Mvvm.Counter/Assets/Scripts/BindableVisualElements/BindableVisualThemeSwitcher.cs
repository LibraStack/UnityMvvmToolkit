using System;
using System.Runtime.CompilerServices;
using BindableUIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableVisualElements
{
    public class BindableVisualThemeSwitcher : TwoWayBindableElement<bool>, IDisposable
    {
        private readonly BindableThemeSwitcher _themeSwitcher;

        public BindableVisualThemeSwitcher(BindableThemeSwitcher themeSwitcher, IProperty<bool> property) :
            base(property)
        {
            _themeSwitcher = themeSwitcher;
            _themeSwitcher.Switch += OnThemeSwitch;
        }

        public void Dispose()
        {
            _themeSwitcher.Switch -= OnThemeSwitch;
        }

        private void OnThemeSwitch(object sender, bool value)
        {
            Property.Value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool TryGetElementValue(out bool value)
        {
            value = _themeSwitcher.IsDarkMode;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void OnPropertyValueChanged(bool newValue)
        {
            _themeSwitcher.SetValueWithoutNotify(newValue);
        }
    }
}