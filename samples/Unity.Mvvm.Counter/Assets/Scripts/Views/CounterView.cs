using UnityEngine;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UI;
using ViewModels;

namespace Views
{
    public class CounterView : DocumentView<CounterViewModel>
    {
        [SerializeField] private AppContext _appContext;

        protected override CounterViewModel GetBindingContext()
        {
            return _appContext.Resolve<CounterViewModel>();
        }

        protected override IBindableVisualElementsCreator GetBindableVisualElementsCreator()
        {
            return _appContext.Resolve<IBindableVisualElementsCreator>();
        }
    }
}