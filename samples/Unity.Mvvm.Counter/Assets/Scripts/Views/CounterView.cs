using UnityEngine;
using UnityMvvmToolkit.UI;
using UnityMvvmToolkit.UI.Interfaces;
using ViewModels;

namespace Views
{
    public class CounterView : View<CounterViewModel>
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