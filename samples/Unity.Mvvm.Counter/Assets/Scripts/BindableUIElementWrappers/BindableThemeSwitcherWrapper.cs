using System;
using BindableUIElements;
using UnityMvvmToolkit.Core;
using UnityMvvmToolkit.Core.Interfaces;

namespace BindableUIElementWrappers
{
    public class BindableThemeSwitcherWrapper : BindablePropertyElement, IInitializable, IDisposable
    {
        private readonly BindableThemeSwitcher _themeSwitcher;
        private readonly IProperty<bool> _valueProperty;

        public BindableThemeSwitcherWrapper(BindableThemeSwitcher themeSwitcher, IObjectProvider objectProvider)
            : base(objectProvider)
        {
            _themeSwitcher = themeSwitcher;
            _valueProperty = GetProperty<bool>(themeSwitcher.BindingValuePath);
        }

        public bool CanInitialize => _valueProperty != null;

        public void Initialize()
        {
            _themeSwitcher.Switch += OnThemeSwitch;
        }

        public override void UpdateValues()
        {
            _themeSwitcher.SetValueWithoutNotify(_valueProperty.Value);
        }

        public void Dispose()
        {
            _themeSwitcher.Switch -= OnThemeSwitch;
        }

        private void OnThemeSwitch(object sender, bool value)
        {
            _valueProperty.Value = value;
        }
    }
}