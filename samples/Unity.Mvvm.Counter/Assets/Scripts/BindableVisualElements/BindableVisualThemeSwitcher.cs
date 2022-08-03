﻿using System;
using BindableUIElements;
using UnityMvvmToolkit.Common;
using UnityMvvmToolkit.Common.Interfaces;

namespace BindableVisualElements
{
    public class BindableVisualThemeSwitcher : BindableVisualElement, IDisposable
    {
        private readonly BindableThemeSwitcher _themeSwitcher;
        private readonly IProperty<bool> _valueProperty;

        public BindableVisualThemeSwitcher(BindableThemeSwitcher themeSwitcher, IPropertyProvider propertyProvider)
            : base(propertyProvider)
        {
            _themeSwitcher = themeSwitcher;
            _themeSwitcher.Switch += OnThemeSwitch;

            _valueProperty = GetProperty<bool>(themeSwitcher.BindingValuePath);
        }

        public void Dispose()
        {
            _themeSwitcher.Switch -= OnThemeSwitch;
        }

        private void OnThemeSwitch(object sender, bool value)
        {
            _valueProperty.Value = value;
        }

        public override void UpdateValues()
        {
            if (_valueProperty != null)
            {
                _themeSwitcher.SetValueWithoutNotify(_valueProperty.Value);
            }
        }
    }
}