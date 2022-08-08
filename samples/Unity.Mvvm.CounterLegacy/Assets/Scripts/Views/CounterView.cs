using UnityMvvmToolkit.Common.Converters.ValueConverters;
using UnityMvvmToolkit.Common.Interfaces;
using UnityMvvmToolkit.UGUI;
using ViewModels;

namespace Views
{
    public class CounterView : CanvasView<CounterViewModel>
    {
        protected override IConverter[] GetValueConverters()
        {
            return new IConverter[] { new IntToStrConverter() };
        }
    }
}