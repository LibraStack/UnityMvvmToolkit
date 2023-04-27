using System;
using System.Collections.Generic;
using UnityEngine;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UITK;

namespace Views
{
    public class BaseView<TBindingContext> : DocumentView<TBindingContext>
        where TBindingContext : class, IBindingContext
    {
        [SerializeField] private AppContext _appContext;

        protected override TBindingContext GetBindingContext()
        {
            return _appContext.Resolve<TBindingContext>();
        }

        protected override IValueConverter[] GetValueConverters()
        {
            return _appContext.Resolve<IValueConverter[]>();
        }

        protected override IReadOnlyDictionary<Type, object> GetCollectionItemTemplates()
        {
            return _appContext.Resolve<IReadOnlyDictionary<Type, object>>();
        }
    }
}