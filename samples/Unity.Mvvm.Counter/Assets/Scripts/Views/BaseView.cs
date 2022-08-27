using System.ComponentModel;
using Interfaces.Services;
using UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UI;

namespace Views
{
    public abstract class BaseView<TBindingContext> : DocumentView<TBindingContext>
        where TBindingContext : class, INotifyPropertyChanged
    {
        [SerializeField] private AppContext _appContext;

        protected override void OnInit()
        {
            base.OnInit();
            RootVisualElement.Q<ContentPage>()?.Initialize(_appContext.Resolve<IThemeService>());
        }

        protected override TBindingContext GetBindingContext()
        {
            return _appContext.Resolve<TBindingContext>();
        }

        protected override IValueConverter[] GetValueConverters()
        {
            return _appContext.Resolve<IValueConverter[]>();
        }

        protected override IBindableElementsWrapper GetBindableElementsWrapper()
        {
            return _appContext.Resolve<IBindableElementsWrapper>();
        }
    }
}