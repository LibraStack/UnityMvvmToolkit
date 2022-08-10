using UnityMvvmToolkit.Core.Converters.PropertyValueConverters;
using UnityMvvmToolkit.Core.Interfaces;
using UnityMvvmToolkit.UGUI;
using ViewModels;

namespace Views
{
    public class CounterView : CanvasView<CounterViewModel>
    {
        protected override IValueConverter[] GetValueConverters()
        {
            return new IValueConverter[] { new IntToStrConverter() };
        }
    }
}