using UnityEngine;
using UnityMvvmToolkit.UITK;
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
    }
}