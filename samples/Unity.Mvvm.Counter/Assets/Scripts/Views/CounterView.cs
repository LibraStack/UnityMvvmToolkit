using System.ComponentModel;
using Enums;
using Services;
using UnityEngine;
using UnityMvvmToolkit.UI;
using UnityMvvmToolkit.UI.Interfaces;
using ViewModels;

namespace Views
{
    public class CounterView : View<CounterViewModel>
    {
        [SerializeField] private ThemeService _themeService;

        protected override CounterViewModel GetBindingContext()
        {
            return new CounterViewModel { IsDarkMode = _themeService.IsDarkMode };
        }

        protected override IBindableVisualElementsCreator<CounterViewModel> GetBindableVisualElementsCreator()
        {
            return new BindableElementsCreator<CounterViewModel>();
        }

        protected override void OnBindingContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BindingContext.IsDarkMode)) // TODO: Bind a view directly to the property?
            {
                _themeService.SetThemeMode(BindingContext.IsDarkMode ? ThemeMode.Dark : ThemeMode.Light);
            }

            base.OnBindingContextPropertyChanged(sender, e);
        }
    }
}