using System.Collections.Generic;
using System.ComponentModel;
using BindableUIElements;
using Interfaces.Services;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI;

namespace Views
{
    public class BaseView<TBindingContext> : DocumentView<TBindingContext>
        where TBindingContext : class, INotifyPropertyChanged
    {
        [SerializeField] private AppContext _appContext;

        protected override TBindingContext GetBindingContext()
        {
            RootVisualElement.Q<BindableRootPage>()?.Initialize(_appContext.Resolve<IThemeService>());
            return _appContext.Resolve<TBindingContext>();
        }

        protected override IEnumerable<IValueConverter> GetValueConverters()
        {
            return _appContext.Resolve<IEnumerable<IValueConverter>>();
        }

        protected override IBindableVisualElementsCreator GetBindableVisualElementsCreator()
        {
            return _appContext.Resolve<IBindableVisualElementsCreator>();
        }
    }
}