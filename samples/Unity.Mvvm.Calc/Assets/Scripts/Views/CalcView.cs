using UnityEngine;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UI;
using ViewModels;

namespace Views
{
    public class CalcView : DocumentView<CalcViewModel>
    {
        [SerializeField] private AppContext _appContext;

        protected override CalcViewModel GetBindingContext()
        {
            return _appContext.Resolve<CalcViewModel>();
        }

        protected override IConverter[] GetValueConverters()
        {
            return _appContext.Resolve<IConverter[]>();
        }
    }
}