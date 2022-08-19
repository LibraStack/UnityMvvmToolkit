using System.ComponentModel;
using Interfaces;
using UnityEngine;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UI;

namespace Views
{
    public class BaseView<TBindingContext> : DocumentView<TBindingContext>
        where TBindingContext : class, INotifyPropertyChanged
    {
        [SerializeField] private AppContext _appContext;

        protected IAppContext AppContext => _appContext;
        
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