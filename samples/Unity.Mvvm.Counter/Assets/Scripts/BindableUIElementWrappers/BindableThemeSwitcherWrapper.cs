using System;
using BindableUIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElementWrappers
{
    public class BindableThemeSwitcherWrapper : BindablePropertyElement, IDisposable
    {
        private readonly BindableThemeSwitcher _themeSwitcher;
        private readonly IProperty<bool> _valueProperty;

        public BindableThemeSwitcherWrapper(BindableThemeSwitcher themeSwitcher, IObjectProvider objectProvider)
            : base(objectProvider)
        {
            _valueProperty = GetProperty<bool>(themeSwitcher.BindingValuePath);

            if (_valueProperty == null)
            {
                return;
            }

            _themeSwitcher = themeSwitcher;
            _themeSwitcher.Switch += OnThemeSwitch;
        }

        public override void UpdateValues()
        {
            _themeSwitcher.SetValueWithoutNotify(_valueProperty.Value);
        }

        public void Dispose()
        {
            if (_valueProperty != null)
            {
                _themeSwitcher.Switch -= OnThemeSwitch;
            }
        }

        private void OnThemeSwitch(object sender, bool value)
        {
            _valueProperty.Value = value;
        }
    }
}