using Interfaces.Services;
using UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK;

namespace Views
{
    public abstract class BaseView<TBindingContext> : DocumentView<TBindingContext>
        where TBindingContext : class, IBindingContext
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

        protected override IObjectProvider GetObjectProvider()
        {
            return _appContext.Resolve<IObjectProvider>();
        }
    }
}